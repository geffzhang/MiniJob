namespace MiniJob.TimeWheel;

/// <summary>
/// 时间轮配置选项
/// </summary>
public class TimingWheelOptions
{
    /// <summary>
    /// 时间槽大小，毫秒，默认值为：1000
    /// </summary>
    public long TickSpan { get; set; } = 1000;

    /// <summary>
    /// 时间槽数量，默认值为：60
    /// </summary>
    public int SlotCount { get; set; } = 60;

    /// <summary>
    /// 起始时间戳，标识时间轮创建时间，默认当前时间
    /// </summary>
    public DateTime StartTimestamp { get; set; } = DateTime.Now;
}
