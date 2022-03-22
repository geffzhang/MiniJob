using MiniJob.DelayQueue;

namespace MiniJob.TimeWheel;

/// <summary>
/// 分层算法时间轮
/// </summary>
internal class TimingWheel
{
    /// <summary>
    /// 时间槽大小，即刻度，毫秒
    /// 注意这个代表时间轮的精度，比如设置为1s，那么所有小于1s的延迟任务都算到期，不管是1ms还是999ms
    /// </summary>
    private readonly long _tickSpan;

    /// <summary>
    /// 时间槽数量
    /// </summary>
    private readonly int _slotCount;

    /// <summary>
    /// 时间轮大小，毫秒(槽大小*槽数量)
    /// </summary>
    private readonly long _wheelSpan;

    /// <summary>
    /// 时间槽
    /// </summary>
    private readonly TimeSlot[] _timeSlots;

    /// <summary>
    /// 当前指针，标识当前时间槽的时间戳，是tickSpan的整数倍
    /// <para>指针指向的时间槽，就是刚好到期的时间槽</para>
    /// <para>当前槽的范围为：[currentNeedle, currentNeedle + tickSpan)</para>
    /// </summary>
    private long _currentNeedle;

    /// <summary>
    /// 下一层时间轮
    /// </summary>
    private TimingWheel _nextWheel;

    /// <summary>
    /// 时间槽延时队列
    /// </summary>
    private readonly DelayQueue<TimeSlot> _delayQueue;

    private readonly object _lock = new object();

    public int TaskCount => GetTaskCount();

    /// <summary>
    /// 分层算法时间轮
    /// </summary>
    /// <param name="tickSpan">时间槽大小，毫秒</param>
    /// <param name="slotCount">时间槽数量</param>
    /// <param name="startMs">起始时间戳，标识时间轮创建时间</param>
    /// <param name="delayQueue">时间槽延时队列</param>
    public TimingWheel(long tickSpan, int slotCount, long startMs, DelayQueue<TimeSlot> delayQueue)
    {
        _tickSpan = tickSpan;
        _slotCount = slotCount;
        _delayQueue = delayQueue;

        _wheelSpan = _tickSpan * _slotCount;
        _timeSlots = new TimeSlot[_slotCount];

        for (int i = 0; i < _timeSlots.Length; i++)
        {
            _timeSlots[i] = new TimeSlot();
        }

        // 计算起始时间槽的起始时间戳，注意起始时间槽并不一定是第0个
        SetNeedle(startMs);
    }

    /// <summary>
    /// 添加任务
    /// </summary>
    /// <param name="task">要添加的任务</param>
    /// <returns>false: 任务到期、过期 true: 成功添加任务</returns>
    public bool AddTask(ITimeTask task)
    {
        // 任务到期、过期
        if (task.TimeoutMs < _currentNeedle + _tickSpan)
        {
            return false;
        }

        // 是否可以放入当前时间轮
        if (task.TimeoutMs < _currentNeedle + _wheelSpan)
        {
            // 任务已经过期，无法添加
            // 计算过期时间戳所属的时间槽
            var tickCount = task.TimeoutMs / _tickSpan;
            var slotIndex = (int)(tickCount % _slotCount);

            var slot = _timeSlots[slotIndex];
            slot.AddTask(task);

            // 设置成功，说明该时间槽已过期出队，需要重新入队
            // 在同一轮循环内，同一个槽的slotTimeoutMs是一样的
            var slotTimeoutMs = tickCount * _tickSpan;
            if (slot.SetExpiration(slotTimeoutMs))
            {
                // 注意这里有个特殊情况：
                // slotTimeoutMs是按照tickSpan裁剪得到的值，可能会小于当前时间，
                // 意味着这里入队的slot已经超时，TimingWheelTimer会将该slot立即出队。
                _delayQueue.TryAdd(slot);
            }

            return true;
        }
        // 超出当前时间轮，则放入下一层
        else
        {
            CreateNextWheel();
            return _nextWheel.AddTask(task);
        }
    }

    /// <summary>
    /// 推进当前时间轮
    /// </summary>
    /// <param name="timestamp">前进到该时间戳</param>
    public void Step(long timestamp)
    {
        // 时间戳已超出tickSpan，所以需要前进
        if (timestamp >= _currentNeedle + _tickSpan)
        {
            // 调整指针到指定时间戳对应的时间槽
            SetNeedle(timestamp);

            // 同时推动下层时间轮前进
            _nextWheel?.Step(timestamp);
        }
    }

    /// <summary>
    /// 创建下一层时间轮
    /// </summary>
    private void CreateNextWheel()
    {
        if (_nextWheel == null)
        {
            lock (_lock)
            {
                // 槽大小是当前时间轮的大小，起始时间是当前时间轮的指针
                if (_nextWheel == null)
                {
                    _nextWheel = new TimingWheel(_wheelSpan, _slotCount, _currentNeedle, _delayQueue);
                }
            }
        }
    }

    /// <summary>
    /// 设置指针
    /// </summary>
    /// <param name="timestamp"></param>
    private void SetNeedle(long timestamp)
    {
        // 修剪为tickSpan的整数倍
        _currentNeedle = timestamp - (timestamp % _tickSpan);
    }

    private int GetTaskCount()
    {
        int count = 0;
        if (_nextWheel != null)
            count += _nextWheel.GetTaskCount();

        count += _timeSlots.Sum(p => p.Count);

        return count;
    }
}
