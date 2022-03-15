using MiniJob.Localization;
using Volo.Abp.Application.Services;

namespace MiniJob;

/* Inherit your application services from this class.
 */
public abstract class MiniJobAppService : ApplicationService
{
    protected MiniJobAppService()
    {
        LocalizationResource = typeof(MiniJobResource);
    }
}