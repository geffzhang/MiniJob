namespace MiniJob.Entities;

/// <summary>
/// 任务优先级
/// </summary>
public enum JobPriority
{
    /// <summary>
    /// 低
    /// </summary>
    Low = 5,

    /// <summary>
    /// 低中
    /// </summary>
    BelowNormal = 10,

    /// <summary>
    /// 中(默认)
    /// </summary>
    Normal = 15,

    /// <summary>
    /// 中高
    /// </summary>
    AboveNormal = 20,

    /// <summary>
    /// 高
    /// </summary>
    High = 25
}