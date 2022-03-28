namespace MiniJob.Processors;

/// <summary>
/// 处理器名称提供者
/// </summary>
public interface IProcessorNameProvider
{
    /// <summary>
    /// 处理器名称，程序集唯一
    /// </summary>
    string Name { get; }
}