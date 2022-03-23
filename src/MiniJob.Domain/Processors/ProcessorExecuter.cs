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

    public async Task<ProcessorResult> RunAsync(ProcessorContext context)
    {
        using (var scope = ServiceScopeFactory.CreateScope())
        {
            var processor = GetProcessor(context, scope);
            if (processor != null)
                return await processor.ExecuteAsync(context);

            return ProcessorResult.ErrorMessage($"executor type {context.ProcessorInfo} not register, executor must implementation {typeof(IProcessor).AssemblyQualifiedName}");
        }
    }

    protected virtual IProcessor GetProcessor(ProcessorContext context, IServiceScope scope)
    {
        var executorType = Options.GetProcessor(context.ProcessorInfo);
        if (executorType != null)
            return (IProcessor)scope.ServiceProvider.GetRequiredService(executorType.ProcessorType);

        return null;
    }
}
