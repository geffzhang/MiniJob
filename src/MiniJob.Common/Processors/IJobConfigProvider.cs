using MiniJob.Entities;

namespace MiniJob.Processors;

public interface IJobConfigProvider
{
    string Name { get; }

    string Description { get; set; }

    ProcessorType ProcessorType { get; set; }

    public TimeExpressionType TimeExpressionType { get; set; }

    public string TimeExpressionValue { get; set; }

    public string JobArgs { get; set; }

    public MisfireStrategy MisfireStrategy { get; set; }
}
