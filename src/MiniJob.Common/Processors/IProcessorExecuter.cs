using Volo.Abp.DependencyInjection;

namespace MiniJob.Processors;

public interface IProcessorExecuter : ITransientDependency
{
    Task<ProcessorResult> RunAsync(ProcessorContext context);
}
