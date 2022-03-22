namespace MiniJob.TimeWheel;

/// <summary>
/// 定时任务
/// </summary>
public interface ITimeTask
{
    /// <summary>
    /// 所属时间槽
    /// </summary>
    TimeSlot TimeSlot { get; set; }

    /// <summary>
    /// 过期时间戳
    /// </summary>
    long TimeoutMs { get; }

    /// <summary>
    /// 任务
    /// </summary>
    ActionOrAsyncFunc ActionTask { get; }

    /// <summary>
    /// 任务状态
    /// </summary>
    TimeTaskStatus TaskStatus { get; }

    /// <summary>
    /// 执行任务
    /// </summary>
    /// <returns></returns>
    Task RunAsync();

    /// <summary>
    /// 取消任务
    /// </summary>
    bool Cancel();
}
