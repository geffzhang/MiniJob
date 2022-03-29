using System.Collections.Immutable;
using Volo.Abp.Collections;

namespace MiniJob.Processors;

/// <summary>
/// 处理器配置选项，用于保存/获取处理器
/// </summary>
public class MiniJobProcessorOptions
{
    private readonly Dictionary<Type, ProcessorConfiguration> _processorConfigurationsByType;
    private readonly Dictionary<string, ProcessorConfiguration> _processorConfigurationsByName;

    public List<JobConfiguration> Jobs { get; set; }

    /// <summary>
    /// Default: true.
    /// </summary>
    public bool IsJobExecutionEnabled { get; set; } = true;

    public MiniJobProcessorOptions()
    {
        _processorConfigurationsByType = new Dictionary<Type, ProcessorConfiguration>();
        _processorConfigurationsByName = new Dictionary<string, ProcessorConfiguration>();
        Jobs = new List<JobConfiguration>();
    }

    /// <summary>
    /// 获取处理器配置
    /// </summary>
    /// <typeparam name="TProcessor">处理器类型</typeparam>
    /// <returns>处理器配置</returns>
    public ProcessorConfiguration GetProcessor<TProcessor>()
    {
        return GetProcessor(typeof(TProcessor));
    }

    /// <summary>
    /// 获取处理器配置
    /// </summary>
    /// <param name="processorType">处理器类型</param>
    /// <returns>处理器配置</returns>
    /// <exception cref="MiniJobException"></exception>
    public ProcessorConfiguration GetProcessor(Type processorType)
    {
        var processorConfiguration = _processorConfigurationsByType.TryGetValue(processorType, out var obj) ? obj : default;

        if (processorConfiguration == null)
        {
            throw new MiniJobException("Undefined Processor for the Processor type: " + processorType.AssemblyQualifiedName);
        }

        return processorConfiguration;
    }

    /// <summary>
    /// 获取处理器配置
    /// </summary>
    /// <param name="name">处理器名称</param>
    /// <returns>处理器配置</returns>
    /// <exception cref="MiniJobException"></exception>
    public ProcessorConfiguration GetProcessor(string name)
    {
        var processorConfiguration = _processorConfigurationsByName.TryGetValue(name, out var obj) ? obj : default;

        if (processorConfiguration == null)
        {
            throw new MiniJobException("Undefined Processor for the Processor name: " + name);
        }

        return processorConfiguration;
    }

    /// <summary>
    /// 获取所有处理器配置
    /// </summary>
    /// <returns>处理器配置列表</returns>
    public IReadOnlyList<ProcessorConfiguration> GetProcessors()
    {
        return _processorConfigurationsByType.Values.ToImmutableList();
    }

    /// <summary>
    /// 添加处理器
    /// </summary>
    /// <typeparam name="TProcessor">处理器类型</typeparam>
    public void AddProcessor<TProcessor>()
    {
        AddProcessor(typeof(TProcessor));
    }

    /// <summary>
    /// 添加处理器
    /// </summary>
    /// <param name="processorType">处理器类型</param>
    public void AddProcessor(Type processorType)
    {
        AddProcessor(new ProcessorConfiguration(processorType));
    }

    /// <summary>
    /// 添加处理器
    /// </summary>
    /// <param name="processorConfiguration">处理器配置</param>
    public void AddProcessor(ProcessorConfiguration processorConfiguration)
    {
        _processorConfigurationsByType[processorConfiguration.ProcessorType] = processorConfiguration;
        _processorConfigurationsByName[processorConfiguration.ProcessorName] = processorConfiguration;
        AddJob(processorConfiguration);
    }

    /// <summary>
    /// 添加任务
    /// </summary>
    /// <param name="processorConfiguration"></param>
    public void AddJob(ProcessorConfiguration processorConfiguration)
    {
        var jobConfigs = JobConfigAttribute.GetJobConfiguration(processorConfiguration.ProcessorType);
        Jobs.AddRange(jobConfigs);
    }
}