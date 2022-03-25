using MiniJob.Dapr.Actors;
using MiniJob.Scheduler;
using Volo.Abp.Domain.Services;

namespace MiniJob.Services.DomainServices;

/// <summary>
/// 任务调度服务
/// </summary>
public class SchedulerService : DomainService
{
    /// <summary>
    /// 将任务从Server派发到Worker
    /// </summary>
    /// <param name="jobInstanceId">任务实例ID</param>
    /// <param name="dueTime">到期时间</param>
    /// <returns></returns>
    public async Task DispatchAsync(Guid jobInstanceId, TimeSpan dueTime)
    {
        await ActorHelper.CreateActor<IJobDispatchActor, JobDispatchActor>(jobInstanceId.ToString())
                .DispatchAsync(dueTime);
    }

    /// <summary>
    /// 重新派发任务实例(不考虑实例当前的状态)
    /// </summary>
    /// <param name="jobInstanceId"></param>
    /// <returns></returns>
    public async Task ReDispatchAsync(Guid jobInstanceId)
    {
        await ActorHelper.CreateActor<IJobDispatchActor, JobDispatchActor>(jobInstanceId.ToString())
                .ReDispatchAsync();
    }

    /// <summary>
    /// 取消派发任务
    /// </summary>
    /// <param name="jobInstanceId">任务实例ID</param>
    /// <returns>true：成功取消 false：取消失败（已派发）</returns>
    public async Task<bool> CancelDispatchAsync(Guid jobInstanceId)
    {
        return await ActorHelper.CreateActor<IJobDispatchActor, JobDispatchActor>(jobInstanceId.ToString())
            .CancelDispatchAsync();
    }
}
