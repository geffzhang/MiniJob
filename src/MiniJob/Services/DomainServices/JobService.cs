using MiniJob.Entities;
using MiniJob.Entities.Jobs;
using Volo.Abp.Domain.Services;

namespace MiniJob.Services.DomainServices;

/// <summary>
/// 任务服务
/// </summary>
public class JobService : DomainService
{
    protected IJobInfoRepository JobInfoRepository { get; }
    protected IJobInstanceRepository JobInstanceRepository { get; }
    protected JobInstanceService JobInstanceService { get; }
    protected SchedulerService DispatchService { get; }

    public JobService(
        IJobInfoRepository jobInfoRepository,
        IJobInstanceRepository jobInstanceRepository,
        SchedulerService dispatchService,
        JobInstanceService jobInstanceService)
    {
        JobInfoRepository = jobInfoRepository;
        JobInstanceRepository = jobInstanceRepository;
        JobInstanceService = jobInstanceService;
        DispatchService = dispatchService;
    }

    /// <summary>
    /// 手动立即运行某个任务，到期时间设为<see cref="TimeSpan.Zero"/>
    /// </summary>
    /// <param name="appId"></param>
    /// <param name="jobId">任务ID</param>
    /// <param name="instanceArgs">任务实例参数（仅 OpenAPI 存在）</param>
    /// <param name="dueTime">延迟时间，单位 毫秒</param>
    /// <returns>任务实例ID</returns>
    public async Task<Guid> RunJobAsync(Guid appId, Guid jobId, string instanceArgs, TimeSpan dueTime)
    {
        var jobInfo = await JobInfoRepository.GetAsync(jobId);

        Logger.LogInformation("[Job-{JobId}] try to run job in app[{AppId}], instanceArgs={InstanceArgs},delay={DueTime} ms.", jobInfo.Id, appId, instanceArgs, dueTime);

        var jobInstance = await JobInstanceRepository
            .InsertAsync(new JobInstance(GuidGenerator.Create(), appId, jobId, Clock.Now + dueTime, instanceArgs));

        await DispatchService.DispatchAsync(jobInstance.Id, dueTime);

        return jobInstance.Id;
    }

    /// <summary>
    /// 删除某个任务
    /// </summary>
    /// <param name="jobId">任务ID</param>
    /// <returns></returns>
    public async Task DeleteJobAsync(Guid jobId)
    {
        await DeleteOrStopJobAsync(jobId, true);
    }

    /// <summary>
    /// 禁用某个任务
    /// </summary>
    /// <param name="jobId">任务ID</param>
    /// <returns></returns>
    public async Task DisableJobAsync(Guid jobId)
    {
        await DeleteOrStopJobAsync(jobId);
    }

    /// <summary>
    /// 启用某个任务
    /// </summary>
    /// <param name="jobId">任务ID</param>
    /// <returns></returns>
    public async Task EnableJobAsync(Guid jobId)
    {
        var jobInfo = await JobInfoRepository.GetAsync(jobId);

        jobInfo.IsEnabled = true;
        jobInfo.CalculateNextTriggerTime(Clock.Now);
    }

    /// <summary>
    /// 停止或删除某个JOB
    /// 秒级任务还要额外停止正在运行的任务实例
    /// </summary>
    /// <param name="jobId">任务ID</param>
    /// <param name="deleteJob">是否删除任务</param>
    /// <returns></returns>
    protected virtual async Task DeleteOrStopJobAsync(Guid jobId, bool deleteJob = false)
    {
        var jobInfo = await JobInfoRepository.GetAsync(jobId);

        // 关闭秒级任务
        if (jobInfo.TimeExpression.IsSecondType())
        {
            var runningInstances = await JobInstanceRepository.GetRuningSecondJobInstanceAsync(jobId);
            if (runningInstances.Any())
            {
                if (runningInstances.Count > 0)
                {
                    Logger.LogWarning("[Job-{JobId}] second job should just have one running instance, actual have {RunningCount} running, there must have some bug.", jobId, runningInstances.Count);
                }

                foreach (var runningInstance in runningInstances)
                {
                    try
                    {
                        await JobInstanceService.StopJobInstanceAsync(runningInstance);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogWarning(ex, "[Job-{JobId}] stop running instance(Id={JobInstanceId}) failed.", runningInstance.Id);
                    }
                }
            }
        }

        // 更新任务运行表
        if (deleteJob)
        {
            await JobInfoRepository.DeleteAsync(jobInfo);
        }
        else
        {
            jobInfo.IsEnabled = false;
        }
    }
}
