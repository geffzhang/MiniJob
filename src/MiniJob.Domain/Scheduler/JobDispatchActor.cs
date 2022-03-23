using Dapr.Actors.Runtime;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MiniJob.Dapr.Actors;
using MiniJob.Enums;
using MiniJob.Jobs;
using MiniJob.Processors;
using Polly;
using System.Net.Http.Json;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;

namespace MiniJob.Scheduler;

/// <summary>
/// 派送服务（将任务从Server派发到Worker）
/// </summary>
public class JobDispatchActor : MiniJobActor, IJobDispatchActor
{
    private const string TimerName = "MiniJobDispatchReminder";
    private bool _isFinished = true;

    protected IJobInstanceRepository JobInstanceRepository { get; }
    protected IJobInfoRepository JobInfoRepository { get; }
    protected IRepository<AppInfo, Guid> AppInfoRepository { get; }
    protected IServiceScopeFactory ServiceScopeFactory { get; }
    protected IProcessorExecuter ProcessorExecuter { get; }
    protected HttpClient HttpClient { get; }

    public JobDispatchActor(
        ActorHost host,
        IJobInstanceRepository jobInstanceRepository,
        IJobInfoRepository jobInfoRepository,
        IRepository<AppInfo, Guid> appInfoRepository,
        IServiceScopeFactory serviceScopeFactory,
        IProcessorExecuter processorExecuter,
        IHttpClientFactory httpClientFactory)
        : base(host)
    {
        JobInstanceRepository = jobInstanceRepository;
        JobInfoRepository = jobInfoRepository;
        AppInfoRepository = appInfoRepository;
        ServiceScopeFactory = serviceScopeFactory;
        ProcessorExecuter = processorExecuter;
        HttpClient = httpClientFactory.CreateClient();
    }

    public async Task DispatchAsync(TimeSpan dueTime)
    {
        // 注册到期时间后执行一次的Timer
        // 如果执行失败，10秒后尝试重试
        await RegisterTimerAsync(TimerName, nameof(TimerCallback), null, dueTime, TimeSpan.FromSeconds(10));
    }

    public async Task ReDispatchAsync()
    {
        await DispatchCoreAsync();
    }

    public async Task<bool> CancelDispatchAsync()
    {
        // todo: 检查是否已执行派发操作
        _isFinished = true;
        await DispatchCompleteAsync();
        return true;
    }

    public async Task TimerCallback()
    {
        try
        {
            await DispatchCoreAsync();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "JobInstance-{JobInstanceId} dispatch failed", Id.GetId());
        }
        finally
        {
            await DispatchCompleteAsync();
        }
    }

    [UnitOfWork]
    protected virtual async Task DispatchCoreAsync()
    {
        var jobInstanceId = Guid.Parse(Id.GetId());
        _isFinished = true;

        // 等待任务实例写入数据库
        var jobInstance = await Policy
            .HandleResult<JobInstance>(jobInstance => jobInstance == null)
            .WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(500))
            .ExecuteAsync(async () => await JobInstanceRepository.FindAsync(jobInstanceId));

        if (jobInstance == null)
        {
            Logger.LogInformation("JobInstance-{JobInstanceId} not found in database.", jobInstanceId);
            return;
        }

        // 检查当前任务是否被取消
        if (jobInstance.InstanceStatus == InstanceStatus.Canceled)
        {
            Logger.LogInformation("{JobInstance} cancel dispatch due to instance has been canceled", jobInstance);
            return;
        }

        if (jobInstance.InstanceStatus != InstanceStatus.WaitingDispatch)
        {
            Logger.LogInformation("{JobInstance} cancel dispatch due to instance has been dispatched", jobInstance);
            return;
        }

        // 任务信息已经被删除
        var jobInfo = await JobInfoRepository.FindAsync(jobInstance.JobInfoId);
        if (jobInfo == null)
        {
            Logger.LogWarning("{JobInstance} cancel dispatch due to has been deleted!", jobInstance);
            return;
        }

        Logger.LogInformation("{JobInstance} start to dispatch job: {JobInfo};instanceArgs: {InstanceArgs}.", jobInstance, jobInfo, jobInstance.InstanceArgs);

        jobInstance.ActualTriggerTime = Clock.Now;
        jobInfo.LastTriggerTime = Clock.Now;

        await DispatchJobInstanceAsync(jobInstance, jobInfo);

        // 延迟任务执行一次后失效
        if (jobInfo.TimeExpression == TimeExpressionType.Delayed)
        {
            jobInfo.IsEnabled = false;
        }
    }

    protected virtual async Task DispatchJobInstanceAsync(JobInstance jobInstance, JobInfo jobInfo)
    {
        if (jobInfo.ExecutorInfo.IsNullOrEmpty())
        {
            Logger.LogWarning("JobInfo-{JobInfo} executorinfo is empty.", jobInfo);
            return;
        }

        var context = GetExecutorContext(jobInstance, jobInfo);

        // 如果在Server运行则不需要下发，直接执行
        if (true)
        {
            await ExecutorRunner(jobInstance, jobInfo, context);
            return;
        }

        var maxInstanceCount = jobInfo.MaxInstanceCount;
        // 秒级任务只派发到一台机器，具体的 MaxInstance 由 TaskTracker 控制
        if (jobInfo.TimeExpression == TimeExpressionType.SecondDelay)
        {
            maxInstanceCount = 1;
        }

        // 0 代表不限制在线任务
        if (maxInstanceCount > 0)
        {
            // 由于不统计 WaitingDispatch，所以这个 runningInstanceCount 不包含本任务自身
            var runningInstanceCount = (await JobInstanceRepository.GetQueryableAsync())
                .Where(p => p.JobInfoId == jobInstance.JobInfoId)
                .Where(p => p.InstanceStatus == InstanceStatus.Runing || p.InstanceStatus == InstanceStatus.WaitingWorkerReceive)
                .Count();
            // 超出最大同时运行限制，不执行调度
            if (runningInstanceCount >= maxInstanceCount)
            {
                Logger.LogWarning("{JobInstance} cancel dispatch job due to too much instance is running ({RunningCount} > {MaxCount}).", jobInstance, runningInstanceCount, maxInstanceCount);

                jobInstance.InstanceStatus = InstanceStatus.Failed;
                jobInstance.ActualTriggerTime = Clock.Now;
                jobInstance.FinishedTime = Clock.Now;
                jobInstance.Result = $"to many instances({runningInstanceCount}>{maxInstanceCount})";

                return;
            }
        }

        // todo: 向 Worker 派送任务
        var request = new HttpRequestMessage(HttpMethod.Post, "")
        {
            Content = JsonContent.Create(context)
        };
        var response = await HttpClient.SendAsync(request);

        // 修改状态
        jobInstance.InstanceStatus = InstanceStatus.WaitingWorkerReceive;
        jobInstance.ActualTriggerTime = Clock.Now;
    }

    protected virtual async Task ExecutorRunner(JobInstance jobInstance, JobInfo jobInfo, ProcessorContext context)
    {
        try
        {
            var executeResult = await ProcessorExecuter.RunAsync(context);

            jobInstance.FinishedTime = Clock.Now;
            jobInstance.Result = executeResult.Message;
            jobInstance.InstanceStatus = executeResult.Success ? InstanceStatus.Succeed : InstanceStatus.Failed;

            if (!executeResult.Success)
            {
                Logger.LogWarning("execute result is unsuccess: {Result}", executeResult.Message);
            }
        }
        catch (Exception ex)
        {
            if (jobInstance.TryCount >= jobInfo.MaxTryCount)
            {
                Logger.LogError(ex, "executor runner retry {TryCount} greater than max trycount {MaxTryCount} failed.",
                    jobInstance.TryCount, jobInfo.MaxTryCount);
                _isFinished = true;
                return;
            }

            jobInstance.TryCount++;
            jobInstance.InstanceStatus = InstanceStatus.Failed;
            jobInstance.Result = $"executor runner failed, retry: {jobInstance.TryCount}, error: {ex.Message}";
            _isFinished = false;
            Logger.LogWarning(ex, "executor runner failed, retry: {TryCount}", jobInstance.TryCount);
        }
    }

    /// <summary>
    /// 调度完成后注销Timer
    /// </summary>
    /// <returns></returns>
    protected virtual async Task DispatchCompleteAsync()
    {
        if (_isFinished)
            await UnregisterTimerAsync(TimerName);
    }

    /// <summary>
    /// 获取 ExecutorContext
    /// </summary>
    /// <param name="jobInstance">任务实例</param>
    /// <param name="jobInfo">任务信息</param>
    /// <returns></returns>
    protected virtual ProcessorContext GetExecutorContext(JobInstance jobInstance, JobInfo jobInfo)
    {
        return new ProcessorContext()
        {
            JobId = jobInfo.Id,
            JobInstanceId = jobInstance.Id,
            JobArgs = jobInfo.JobArgs,
            InstanceArgs = jobInstance.InstanceArgs,
            MaxTryCount = jobInfo.MaxTryCount,
            TryCount = 0,
            TaskId = Guid.NewGuid(),
            ProcessorType = jobInfo.ProcessorType,
            ProcessorInfo = jobInfo.ExecutorInfo,
        };
    }
}
