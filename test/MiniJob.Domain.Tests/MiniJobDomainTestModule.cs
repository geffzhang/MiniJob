using MiniJob.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace MiniJob
{
    [DependsOn(
        typeof(MiniJobEntityFrameworkCoreTestModule)
        )]
    public class MiniJobDomainTestModule : AbpModule
    {

    }
}