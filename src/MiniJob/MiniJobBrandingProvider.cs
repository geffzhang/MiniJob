using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace MiniJob;

[Dependency(ReplaceServices = true)]
public class MiniJobBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "MiniJob";
}
