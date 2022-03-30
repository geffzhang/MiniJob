using MiniJob.Entities;

namespace MiniJob.Processors;

public interface IJobConfigProvider
{
    string Name { get; }

    string Description { get; set; }

    TimeExpressionType TimeExpressionType { get; set; }

    string TimeExpressionValue { get; set; }

    string JobArgs { get; set; }

    MisfireStrategy MisfireStrategy { get; set; }

    ExecuteType ExecuteType { get; set; }

    JobPriority JobPriority { get; set; }

    bool IsEnabled { get; set; }
}
