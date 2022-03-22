using MiniJob.DelayQueue;

namespace MiniJob.TimeWheel;

/// <summary>
/// 时间槽
/// </summary>
public class TimeSlot : LinkedList<ITimeTask>, IDelayItem
{
    private readonly object _lock = new();

    private long _timeoutMs;
    public long TimeoutMs => _timeoutMs;

    public void AddTask(ITimeTask task)
    {
        var done = false;
        while (!done)
        {
            lock (_lock)
            {
                AddLast(task);
                task.TimeSlot = this;
                done = true;
            }
        }
    }

    /// <summary>
    /// 输出所有任务
    /// </summary>
    /// <param name="func"></param>
    public void Flush(Func<ITimeTask, Task> func)
    {
        lock (_lock)
        {
            while (Count > 0)
            {
                var task = Last;

                Remove(task);
                task.Value.TimeSlot = null;
                func(task.Value);
            }

            // 重置过期时间，标识该时间槽已出队
            _timeoutMs = default;
        }
    }

    /// <summary>
    /// 设置过期时间
    /// </summary>
    /// <param name="timeoutMs"></param>
    /// <returns></returns>
    public bool SetExpiration(long timeoutMs)
    {
        // 第一次设置槽的时间，或是复用槽时，两者才不相等
        return Interlocked.Exchange(ref _timeoutMs, timeoutMs) != timeoutMs;
    }

    public TimeSpan GetDelaySpan()
    {
        var delayMs = Math.Max(TimeoutMs - GetNowTimestamp(), 0);
        return TimeSpan.FromMilliseconds(delayMs);
    }

    public int CompareTo(object obj)
    {
        if (obj == null)
        {
            return 1;
        }

        if (obj is TimeSlot slot)
        {
            return TimeoutMs.CompareTo(slot.TimeoutMs);
        }

        throw new ArgumentException($"Object is not a {nameof(TimeSlot)}");
    }

    public static long GetNowTimestamp(bool isSecond = false)
    {
        var dateTimeOffset = new DateTimeOffset(DateTime.Now);

        return isSecond
            ? dateTimeOffset.ToUnixTimeSeconds()
            : dateTimeOffset.ToUnixTimeMilliseconds();
    }
}
