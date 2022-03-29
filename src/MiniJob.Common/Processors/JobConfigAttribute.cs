using MiniJob.Entities;

namespace MiniJob.Processors;

/// <summary>
/// 任务配置，在任务处理器加上此注解
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class JobConfigAttribute : Attribute, IJobConfigProvider
{
    public string Name { get; }

    public string Description { get; set; }

    public ProcessorType ProcessorType { get; set; }

    public TimeExpressionType TimeExpressionType { get; set; }

    public string TimeExpressionValue { get; set; }

    public string JobArgs { get; set; }

    public MisfireStrategy MisfireStrategy { get; set; }

    public JobConfigAttribute(string name)
    {
        Name = name;
        ProcessorType = ProcessorType.CSharp;
        TimeExpressionType = TimeExpressionType.Api;
        MisfireStrategy = MisfireStrategy.Ignore;
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
                ProcessorType = p.ProcessorType,
                TimeExpressionType = p.TimeExpressionType,
                TimeExpressionValue = p.TimeExpressionValue,
                Type = processorType,
                Description = p.Description
            });
    }
}
