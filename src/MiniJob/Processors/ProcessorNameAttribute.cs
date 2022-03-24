namespace MiniJob.Processors;

/// <summary>
/// 处理器名称
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class ProcessorNameAttribute : Attribute, IProcessorNameProvider
{
    public string Name { get; }

    public ProcessorNameAttribute(string name)
    {
        Name = name;
    }

    /// <summary>
    /// 获取处理器名称，默认为类型全称，可用 <see cref="ProcessorNameAttribute"/> 更改
    /// </summary>
    /// <typeparam name="TProcessor">处理器类型</typeparam>
    /// <returns>处理器名称</returns>
    public static string GetName<TProcessor>()
    {
        return GetName(typeof(TProcessor));
    }

    /// <summary>
    /// 获取处理器名称，默认为类型全称，可用 <see cref="ProcessorNameAttribute"/> 更改
    /// </summary>
    /// <param name="processorType">处理器类型</param>
    /// <returns>处理器名称</returns>
    public static string GetName(Type processorType)
    {
        return processorType
            .GetCustomAttributes(true)
            .OfType<IProcessorNameProvider>()
            .FirstOrDefault()
            ?.Name
        ?? processorType.FullName;
    }
}