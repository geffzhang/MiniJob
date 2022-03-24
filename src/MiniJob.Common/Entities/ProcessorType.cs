namespace MiniJob.Entities;

/// <summary>
/// 执行器类型
/// </summary>
public enum ProcessorType
{
    /// <summary>
    /// C#
    /// </summary>
    CSharp,

    /// <summary>
    /// 脚本
    /// </summary>
    Shell,

    /// <summary>
    /// Python
    /// </summary>
    Python,

    /// <summary>
    /// Http
    /// </summary>
    Http,

    /// <summary>
    /// SQL语句
    /// </summary>
    SQL
}