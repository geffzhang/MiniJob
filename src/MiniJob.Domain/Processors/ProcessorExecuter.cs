using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace MiniJob.Processors;

internal class ProcessorExecuter : IProcessorExecuter
{
    protected MiniJobProcessorOptions Options { get; }

    protected IServiceScopeFactory ServiceScopeFactory { get; }

    public ProcessorExecuter(
    IOptions<MiniJobProcessorOptions> options,
    IServiceScopeFactory serviceScopeFactory)
    {
        Options = options.Value;
        ServiceScopeFactory = serviceScopeFactory;
    }

    public Task<ProcessorResult> RunAsync(ProcessorContext context)
    {
        throw new NotImplementedException();
    }

    protected virtual IProcessor GetJobExecutor(ProcessorContext context, IServiceScope scope)
    {
        var executorType = Options.GetProcessor(context.ExecutorInfo);
        if (executorType != null)
            return (IProcessor)scope.ServiceProvider.GetRequiredService(executorType.ProcessorType);

        return null;
    }
}
