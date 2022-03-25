using Dapr.Actors.Runtime;
using Microsoft.Extensions.Options;
using MiniJob.Dapr.Actors;
using MiniJob.Entities;
using MiniJob.Entities.Jobs;
using MiniJob.Services.DomainServices;
using Volo.Abp.Uow;

namespace MiniJob.Scheduler;

public class JobInstanceStatusCheckActor : MiniJobActor, IJobInstanceStatusCheckActor
{
    public readonly TimeSpan DispatchTimeout = TimeSpan.FromMilliseconds(30000);
    public readonly TimeSpan ReceiveTimeout = TimeSpan.FromMilliseconds(60000);
    public readonly TimeSpan RunningTimeout = TimeSpan.FromMilliseconds(60000);

    protected MiniJobSchedulerOptions MiniJobOptions { get; }
    protected IJobInfoRepository JobInfoRepository { get; }
    protected IJobInstanceRepository JobInstanceRepository { get; }
    protected SchedulerService JobSchedulerService { get; }
    protected JobInstanceService JobInstanceService { get; }

    public JobInstanceStatusCheckActor(
        ActorHost host,
        IOptions<MiniJobSchedulerOptions> options,
        IJobInfoRepository jobInfoRepository,
        IJobInstanceRepository jobInstanceRepository,
        SchedulerService jobSchedulerService,
        JobInstanceService jobInstanceService)
        : base(host)
    {
        MiniJobOptions = options.Value;
        JobInfoRepository = jobInfoRepository;
        JobInstanceRepository = jobInstanceRepository;
        JobSchedulerService = jobSchedulerService;
        JobInstanceService = jobInstanceService;
    }

    [UnitOfWork]
    public async Task InstanceStatusCheckAsync()
    {
        // 检查 WaitingDispatch 状态的任务实例
        await HandleWaitingDispatchInstanceAsync();

        // 检查 WaitingWorkerReceive 状态的任务实例(一定时间内Worker没有接收成功)
        await HandleWaitingWorkerReceiveInstanceAsync();

        // 检查 Running 状态的任务实例(一定时间内没有收到状态报告)
        await HandleRuningInstanceAsync();
    }

    public async Task HandleWaitingDispatchInstanceAsync()
    {
        var waitingDispatchInstances = await JobInstanceRepository
            .GetListAsync(p => p.InstanceStatus == InstanceStatus.WaitingDispatch &&
                p.ExpectedTriggerTime < Clock.Now - DispatchTimeout);
        if (waitingDispatchInstances.Any())
        {
            Logger.LogWarning("find {Count} instance which is not triggered as expected: {JobInstance}, now try to redispatch.",
                waitingDispatchInstances.Count, waitingDispatchInstances);

            foreach (var waitingInstance in waitingDispatchInstances)
            {
                await JobSchedulerService.ReDispatchAsync(waitingInstance.Id);
            }
        }
    }

    public async Task HandleWaitingWorkerReceiveInstanceAsync()
    {
        var waitingWorkerReceiveInstances = await JobInstanceRepository
            .GetListAsync(p => p.InstanceStatus == InstanceStatus.WaitingWorkerReceive &&
                p.ActualTriggerTime < Clock.Now - ReceiveTimeout);
        if (waitingWorkerReceiveInstances.Any())
        {
            Logger.LogWarning("find {Count} instance didn't receive any reply from worker, try to redispatch: {JobInstance}",
                waitingWorkerReceiveInstances.Count, waitingWorkerReceiveInstances);

            foreach (var waitingInstance in waitingWorkerReceiveInstances)
            {
                await JobSchedulerService.ReDispatchAsync(waitingInstance.Id);
            }
        }
    }

    public async Task HandleRuningInstanceAsync()
    {
        var failedInstances = await JobInstanceRepository
            .GetListAsync(p => p.InstanceStatus == InstanceStatus.Runing &&
                p.LastReportTime < Clock.Now - RunningTimeout);
        if (failedInstances.Any())
        {
            Logger.LogWarning("instances({JobInstance}) has not received status report for {Timeout}.", failedInstances, RunningTimeout);

            foreach (var failedInstance in failedInstances)
            {
                var jobInfo = await JobInfoRepository.GetAsync(failedInstance.JobInfoId);

                // 如果任务已关闭，则不进行重试，将任务置为失败即可；秒级任务也直接置为失败，由派发器重新调度
                if (!jobInfo.IsEnabled || jobInfo.TimeExpression == TimeExpressionType.SecondDelay)
                {
                    await UpdateFailedJobInstance(failedInstance, MiniJobConsts.ReportTimeout);
                }

                // CRON 和 API一样，失败次数 + 1，根据重试配置进行重试
                if (failedInstance.TryCount < jobInfo.MaxTryCount)
                {
                    await JobSchedulerService.ReDispatchAsync(failedInstance.Id);
                }
                else
                {
                    await UpdateFailedJobInstance(failedInstance, MiniJobConsts.ReportTimeout);
                }
            }
        }
    }

    protected virtual async Task UpdateFailedJobInstance(JobInstance jobInstance, string result)
    {
        Logger.LogWarning("{JobInstance} failed due to {Result}", jobInstance, result);

        jobInstance.InstanceStatus = InstanceStatus.Failed;
        jobInstance.FinishedTime = Clock.Now;
        jobInstance.Result = result;

        await JobInstanceService.FinishedInstanceAsync(jobInstance.Id, InstanceStatus.Failed, result);
    }
}
