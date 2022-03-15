namespace MiniJob.Enums;

/// <summary>
/// 执行方式
/// </summary>
public enum ExecuteType
{
    /// <summary>
    /// 单机执行
    /// </summary>
    Standalone,

    /// <summary>
    /// 广播执行
    /// </summary>
    Broadcast,

    /// <summary>
    /// MapReduce
    /// </summary>
    MapReduce
}