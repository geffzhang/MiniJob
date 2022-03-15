using MiniJob.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Modularity;

namespace MiniJob.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(MiniJobEntityFrameworkCoreModule),
    typeof(MiniJobApplicationContractsModule)
    )]
public class MiniJobDbMigratorModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpBackgroundJobOptions>(options => options.IsJobExecutionEnabled = false);
    }
}