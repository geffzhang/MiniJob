using Microsoft.Extensions.DependencyInjection;
using MiniJob.Processors;
using Volo.Abp.Modularity;
using Volo.Abp.Timing;

namespace MiniJob;

[DependsOn(typeof(AbpTimingModule))]
public class MiniJobCommonModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        AutoRegisterProcessors(context.Services);
    }

    private static void AutoRegisterProcessors(IServiceCollection services)
    {
        var processorTypes = new List<Type>();

        services.OnRegistred(context =>
        {
            if (typeof(IProcessor).IsAssignableFrom(context.ImplementationType))
            {
                processorTypes.AddIfNotContains(context.ImplementationType);
            }
        });

        services.Configure<MiniJobProcessorOptions>(options =>
        {
            foreach (var processorType in processorTypes)
            {
                options.AddProcessor(processorType);
            }
        });
    }
}
