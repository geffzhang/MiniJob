using Volo.Abp.Modularity;
using Volo.Abp.Timing;

namespace MiniJob;

[DependsOn(typeof(AbpTimingModule))]
public class MiniJobCommonModule : AbpModule
{

}
