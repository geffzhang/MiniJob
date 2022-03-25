using MiniJob.TimeWheel;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace MiniJob.HttpApi.Client.ConsoleTestApp;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(MiniJobCommonModule)
    )]
public class MiniJobConsoleApiClientModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<TimingWheelOptions>(options =>
        {
            options.TickSpan = 1000;
        });
    }
}