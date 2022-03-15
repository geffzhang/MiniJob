using MiniJob.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace MiniJob.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class MiniJobController : AbpControllerBase
{
    protected MiniJobController()
    {
        LocalizationResource = typeof(MiniJobResource);
    }
}