namespace MiniJob.TimeWheel;

/// <summary>
/// 定时器
/// </summary>
public interface ITimer
{
    /// <summary>
    /// 任务总数
    /// </summary>
    int TaskCount { get; }

    /// <summary>
    ///  调度定时任务
    /// </summary>
    /// <param name="span"></param>
    /// <param name="task"></param>
    Task<ITimeTask> Schedule(TimeSpan span, Action task);

    /// <summary>
    /// 调度定时任务
    /// </summary>
    /// <param name="span"></param>
    /// <param name="task"></param>
    /// <returns></returns>
    Task<ITimeTask> Schedule(TimeSpan span, Func<Task> task);

    /// <summary>
    /// 停止所有调度任务
    /// </summary>
    /// <returns></returns>
    void Stop();
}
