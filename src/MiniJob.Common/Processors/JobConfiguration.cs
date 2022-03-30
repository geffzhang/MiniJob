using MiniJob.Entities;

namespace MiniJob.Processors;

/// <summary>
/// 任务配置
/// </summary>
public class JobConfiguration
{
    public string Name { get; set; }

    public string Description { get; set; }

    public TimeExpressionType TimeExpressionType { get; set; }

    public string TimeExpressionValue { get; set; }

    public string JobArgs { get; set; }

    public MisfireStrategy MisfireStrategy { get; set; }

    public ExecuteType ExecuteType { get; set; }

    public JobPriority JobPriority { get; set; }

    public bool IsEnabled { get; set; }

    public Type ProcessorType { get; set; }
}
