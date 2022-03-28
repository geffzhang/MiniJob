namespace MiniJob.Processors;

/// <summary>
/// 处理器配置，包含处理器类型和名称属性
/// </summary>
public class ProcessorConfiguration
{
    /// <summary>
    /// 处理器类型
    /// </summary>
    public Type ProcessorType { get; }

    /// <summary>
    /// 处理器名称
    /// </summary>
    public string ProcessorName { get; }

    public ProcessorConfiguration(Type processorType)
    {
        ProcessorType = processorType;
        ProcessorName = ProcessorNameAttribute.GetName(processorType);
    }
}