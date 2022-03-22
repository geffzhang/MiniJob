using Microsoft.Extensions.Options;
using MiniJob.DelayQueue;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Timing;

namespace MiniJob.TimeWheel;

/// <summary>
/// 分层算法时间轮计时器
/// </summary>
public class TimingWheelTimer : ITimer, ISingletonDependency
{
    /// <summary>
    /// 时间槽延时队列，和时间轮共用
    /// </summary>
    private readonly DelayQueue<TimeSlot> _delayQueue = new DelayQueue<TimeSlot>();

    /// <summary>
    /// 时间轮
    /// </summary>
    private readonly TimingWheel _timingWheel;

    public int TaskCount => _timingWheel.TaskCount;

    private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
    private volatile CancellationTokenSource _cancelTokenSource;
    private readonly IClock _clock;

    /// <summary>
    /// 分层算法时间轮计时器
    /// </summary>
    /// <param name="options">时间槽大小，毫秒</param>
    /// <param name="clock">时间槽数量</param>
    public TimingWheelTimer(IOptions<TimingWheelOptions> options, IClock clock)
    {
        _clock = clock;
        _timingWheel = new TimingWheel(options.Value.TickSpan, options.Value.SlotCount, GetTimestamp(options.Value.StartTimestamp), _delayQueue);
        
        _cancelTokenSource = new CancellationTokenSource();

        // 时间轮运行线程
        Task.Factory.StartNew(() => Run(_cancelTokenSource.Token),
            _cancelTokenSource.Token,
            TaskCreationOptions.LongRunning,
            TaskScheduler.Default);
    }

    public Task<ITimeTask> Schedule(TimeSpan span, Action task)
    {
        return Schedule(new ActionOrAsyncFunc(task), span);
    }

    public Task<ITimeTask> Schedule(TimeSpan span, Func<Task> task)
    {
        return Schedule(new ActionOrAsyncFunc(task), span);
    }

    protected async Task<ITimeTask> Schedule(ActionOrAsyncFunc actionTask, TimeSpan span)
    {
        Check.NotNull(actionTask, nameof(actionTask));

        _lock.EnterReadLock();
        try
        {
            var task = new TimeTask(actionTask, GetTimestamp(_clock.Now) + (long)span.TotalMilliseconds);
            await AddTask(task);
            return task;
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    public void Stop()
    {
        if (_cancelTokenSource != null)
        {
            _cancelTokenSource.Cancel();
            _cancelTokenSource.Dispose();
            _cancelTokenSource = null;
        }
        _delayQueue.Clear();
    }

    /// <summary>
    /// 运行
    /// </summary>
    private void Run(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                Step(token);
            }
        }
        catch (Exception e)
        {
            if (e is OperationCanceledException)
            {
                return;
            }

            throw;
        }
    }

    /// <summary>
    /// 推进时间轮
    /// </summary>
    /// <param name="token"></param>
    private void Step(CancellationToken token)
    {
        // 阻塞式获取，到期的时间槽才会出队
        if (_delayQueue.TryTake(out var slot, token))
        {
            _lock.EnterWriteLock();
            try
            {
                while (!token.IsCancellationRequested)
                {
                    // 推进时间轮
                    _timingWheel.Step(slot.TimeoutMs);

                    // 到期的任务会重新添加进时间轮，那么下一层时间轮的任务重新计算后可能会进入上层时间轮。
                    // 这样就实现了任务在时间轮中的传递，由大精度的时间轮进入小精度的时间轮。
                    slot.Flush(AddTask);

                    // Flush之后可能有新的slot入队，可能仍旧过期，因此尝试继续处理，直到没有过期项。
                    if (!_delayQueue.TryTakeNoBlocking(out slot))
                    {
                        break;
                    }
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }
    }

    /// <summary>
    /// 添加任务
    /// </summary>
    /// <param name="timeTask">延时任务</param>
    private async Task AddTask(ITimeTask timeTask)
    {
        // 添加失败，说明该任务已到期，需要执行了
        if (!_timingWheel.AddTask(timeTask) && timeTask.TaskStatus == TimeTaskStatus.Waiting)
        {
            await timeTask.RunAsync();
        }
    }

    /// <summary>
    /// 获取时间戳
    /// </summary>
    /// <param name="dateTime"></param>
    /// <param name="isSecond"></param>
    /// <returns></returns>
    private static long GetTimestamp(DateTime dateTime, bool isSecond = false)
    {
        var dateTimeOffset = new DateTimeOffset(dateTime);

        return isSecond
            ? dateTimeOffset.ToUnixTimeSeconds()
            : dateTimeOffset.ToUnixTimeMilliseconds();
    }
}
