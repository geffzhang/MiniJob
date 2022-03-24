namespace MiniJob.TimeWheel;

public class TimeTask : ITimeTask
{
    public TimeSlot TimeSlot { get; set; }

    public long TimeoutMs { get; }

    public ActionOrAsyncFunc ActionTask { get; }

    public TimeTaskStatus TaskStatus { get; private set; } = TimeTaskStatus.Waiting;

    public bool IsWaiting => TaskStatus == TimeTaskStatus.Waiting;

    public TimeTask(ActionOrAsyncFunc task, long timeoutMs)
    {
        ActionTask = task;
        TimeoutMs = timeoutMs;
    }

    /// <summary>
    /// 执行任务
    /// </summary>
    public async Task RunAsync()
    {
        if (!IsWaiting)
        {
            return;
        }

        TaskStatus = TimeTaskStatus.Running;
        Remove();

        try
        {
            await ActionTask.InvokeAsync();
            TaskStatus = TimeTaskStatus.Success;
        }
        catch
        {
            // 由DelayTask内部处理异常，这里不处理
            TaskStatus = TimeTaskStatus.Fail;
        }
    }

    public bool Cancel()
    {
        if (IsWaiting)
        {
            TaskStatus = TimeTaskStatus.Cancel;
            Remove();
            return true;
        }
        return false;
    }

    /// <summary>
    /// 移除任务
    /// </summary>
    internal void Remove()
    {
        while (TimeSlot != null && !TimeSlot.Remove(this))
        {
            // 如果task被另一个线程移动到了其它bucket中，就会移除失败，需要重试
        }

        TimeSlot = null;
    }
}
