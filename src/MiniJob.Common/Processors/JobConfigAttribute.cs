using MiniJob.Entities;

namespace MiniJob.Processors;

/// <summary>
/// 任务配置，在任务处理器加上此注解
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class JobConfigAttribute : Attribute, IJobConfigProvider
{
    public string Name { get; }

    public string Description { get; set; }

    public TimeExpressionType TimeExpressionType { get; set; }

    public string TimeExpressionValue { get; set; }

    public string JobArgs { get; set; }

    public MisfireStrategy MisfireStrategy { get; set; }

    public ExecuteType ExecuteType { get; set; }

    public JobPriority JobPriority { get; set; }

    public bool IsEnabled { get; set; }

    public JobConfigAttribute(string name)
    {
        Name = name;
        TimeExpressionType = TimeExpressionType.Api;
        MisfireStrategy = MisfireStrategy.Ignore;
        ExecuteType = ExecuteType.Standalone;
        JobPriority = JobPriority.Normal;
        IsEnabled = true;
    }

    public static IEnumerable<JobConfiguration> GetJobConfiguration<TProcessor>()
    {
        return GetJobConfiguration(typeof(TProcessor));
    }

    public static IEnumerable<JobConfiguration> GetJobConfiguration(Type processorType)
    {
        return processorType
            .GetCustomAttributes(false)
            .OfType<IJobConfigProvider>()
            .Select(p => new JobConfiguration
            {
                Name = p.Name,
                JobArgs = p.JobArgs,
                MisfireStrategy = p.MisfireStrategy,
                TimeExpressionType = p.TimeExpressionType,
                TimeExpressionValue = p.TimeExpressionValue,
                ProcessorType = processorType,
                Description = p.Description,
                ExecuteType = p.ExecuteType,
                JobPriority = p.JobPriority,
                IsEnabled = p.IsEnabled
            });
    }
}
