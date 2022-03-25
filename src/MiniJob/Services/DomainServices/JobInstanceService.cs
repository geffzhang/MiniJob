using MiniJob.Entities;
using MiniJob.Entities.Jobs;
using Volo.Abp.Domain.Services;
using Volo.Abp.Users;

namespace MiniJob.Services.DomainServices;

/// <summary>
/// 任务运行实例服务
/// </summary>
public class JobInstanceService : DomainService
{
    protected IJobInstanceRepository JobInstanceRepository { get; }
    protected IJobInfoRepository JobInfoRepository { get; }
    protected ICurrentUser CurrentUser { get; }
    protected SchedulerService DispatchService { get; }

    public JobInstanceService(
        IJobInstanceRepository jobInstanceRepository,
        IJobInfoRepository jobInfoRepository,
        SchedulerService dispatchService,
        ICurrentUser currentUser)
    {
        JobInstanceRepository = jobInstanceRepository;
        CurrentUser = currentUser;
        JobInfoRepository = jobInfoRepository;
        DispatchService = dispatchService;
    }

    /// <summary>
    /// 停止任务实例
    /// </summary>
    /// <param name="instanceId">任务实例ID</param>
    /// <returns></returns>
    public async Task StopJobInstanceAsync(Guid instanceId)
    {
        var jobInstance = await JobInstanceRepository.GetAsync(instanceId);
        await StopJobInstanceAsync(jobInstance);
    }

    /// <summary>
    /// 停止任务实例
    /// </summary>
    /// <param name="jobInstance">任务实例</param>
    /// <returns></returns>
    public async Task StopJobInstanceAsync(JobInstance jobInstance)
    {
        Logger.LogInformation("[Instance-{JobInstanceId}] try to stop the instance instance in appId: {AppId}", jobInstance.Id, jobInstance.AppId);

        try
        {
            if (jobInstance.InstanceStatus.IsRunning())
                throw new MiniJobException("can't stop finished instance!");

            jobInstance.InstanceStatus = InstanceStatus.Stoped;
            jobInstance.FinishedTime = Clock.Now;
            jobInstance.Result = $"stopped by user {CurrentUser?.UserName}";

            await JobInstanceRepository.UpdateAsync(jobInstance, true);

            await FinishedInstanceAsync(jobInstance.Id, InstanceStatus.Stoped, jobInstance.Result);

            // todo: 通知 Worker 关闭秒级任务
        }
        catch (MiniJobException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "[Instance-{JobInstanceId}] stopInstance failed.", jobInstance.Id);
            throw;
        }
    }

    /// <summary>
    /// 重试任务（结束的任务运行重试）
    /// </summary>
    /// <param name="instanceId">任务实例ID</param>
    /// <returns></returns>
    public async Task RetryJobInstanceAsync(Guid instanceId)
    {
        var jobInstance = await JobInstanceRepository.GetAsync(instanceId);
        await RetryJobInstanceAsync(jobInstance);
    }

    /// <summary>
    /// 重试任务（结束的任务运行重试）
    /// </summary>
    /// <param name="jobInstance">任务实例</param>
    /// <returns></returns>
    public async Task RetryJobInstanceAsync(JobInstance jobInstance)
    {
        Logger.LogInformation("[Instance-{JobInstanceId}] retry instance in appId: {AppId}", jobInstance.Id, jobInstance.AppId);

        jobInstance.InstanceStatus = InstanceStatus.WaitingDispatch;
        jobInstance.ExpectedTriggerTime = Clock.Now;
        jobInstance.FinishedTime = null;
        jobInstance.ActualTriggerTime = null;
        jobInstance.Result = null;

        await DispatchService.DispatchAsync(jobInstance.Id, TimeSpan.Zero);
    }

    /// <summary>
    /// 取消任务实例的运行
    /// 注意：调用接口时间与待取消任务的预计执行时间有一定时间间隔，否则不保证可靠性！
    /// </summary>
    /// <param name="instanceId">任务实例ID</param>
    /// <returns></returns>
    public async Task CancelJobInstanceAsync(Guid instanceId)
    {
        var jobInstance = await JobInstanceRepository.GetAsync(instanceId);
        await CancelJobInstanceAsync(jobInstance);
    }

    /// <summary>
    /// 取消任务实例的运行
    /// 注意：调用接口时间与待取消任务的预计执行时间有一定时间间隔，否则不保证可靠性！
    /// </summary>
    /// <param name="jobInstance">任务实例</param>
    /// <returns></returns>
    public async Task CancelJobInstanceAsync(JobInstance jobInstance)
    {
        Logger.LogInformation("[Instance-{JobInstanceId}] try to cancel the instance in appId: {AppId}", jobInstance.Id, jobInstance.AppId);

        try
        {
            var isSuccess = await DispatchService.CancelDispatchAsync(jobInstance.Id);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "[Instance-{JobInstanceId}] cancel Instance failed.", jobInstance.Id);
            throw;
        }
    }

    /// <summary>
    /// 完成任务实例
    /// </summary>
    /// <param name="jobInstanceId">任务实例ID</param>
    /// <param name="instanceStatus">任务状态</param>
    /// <param name="result">执行结果</param>
    /// <returns></returns>
    public async Task FinishedInstanceAsync(Guid jobInstanceId, InstanceStatus instanceStatus, string result)
    {
        Logger.LogInformation("[Instance-{JobInstanceId}] execute finished, final status is {InstanceStatus}.", jobInstanceId, instanceStatus);

        // todo: 日志数据处理

        // todo: 告警
        if (instanceStatus == InstanceStatus.Failed)
        {
            var jobInstance = await JobInstanceRepository.GetAsync(jobInstanceId);
            var jobInfo = await JobInfoRepository.GetAsync(jobInstance.JobInfoId);
        }
    }
}
