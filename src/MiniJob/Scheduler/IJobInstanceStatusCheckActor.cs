using Dapr.Actors;

namespace MiniJob.Scheduler;

public interface IJobInstanceStatusCheckActor : IActor
{
    /// <summary>
    /// 检查任务实例的状态，发现异常及时重试，包括
    /// WAITING_WORKER_RECEIVE 超时：由于网络错误导致 worker 未接受成功
    /// RUNNING 超时：TaskTracker down，断开与 server 的心跳连接
    /// </summary>
    /// <returns></returns>
    Task InstanceStatusCheckAsync();
}
