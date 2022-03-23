using Dapr.Actors;

namespace MiniJob.Scheduler;

/// <summary>
/// 派送Actor(将任务从Server派发到Worker)
/// 只会派发当前状态为等待派发的任务实例
/// </summary>
public interface IJobDispatchActor : IActor
{
    /// <summary>
    /// 将任务从Server派发到Worker（TaskTracker）
    /// </summary>
    /// <param name="dueTime">到期时间</param>
    /// <returns></returns>
    Task DispatchAsync(TimeSpan dueTime);

    /// <summary>
    /// 取消派发任务，不能保证一定成功，
    /// </summary>
    /// <returns>true：成功取消 false：取消失败（已派发）</returns>
    Task<bool> CancelDispatchAsync();

    /// <summary>
    /// 重新派发任务实例(不考虑实例当前的状态)
    /// </summary>
    /// <returns></returns>
    Task ReDispatchAsync();
}
