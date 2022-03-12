using Volo.Abp.Modularity;

namespace MiniJob
{
    [DependsOn(
        typeof(MiniJobApplicationModule),
        typeof(MiniJobDomainTestModule)
        )]
    public class MiniJobApplicationTestModule : AbpModule
    {

    }
}