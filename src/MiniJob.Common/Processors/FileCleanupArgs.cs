namespace MiniJob.Processors;

/// <summary>
/// 文件清理参数
/// </summary>
public class FileCleanupArgs
{
    /// <summary>
    /// 文件路径
    /// </summary>
    public virtual string DirPath { get; set; }

    /// <summary>
    /// 文件搜索匹配模式
    /// </summary>
    public virtual string SearchPattern { get; set; }

    /// <summary>
    /// 保留时间（小时）
    /// </summary>
    public virtual int RetentionTime { get; set; }

    /// <summary>
    /// 是否包含子目录，默认为 false
    /// </summary>
    public virtual bool IncludeSubDir { get; set; }
}
