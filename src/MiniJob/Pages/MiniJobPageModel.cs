using MiniJob.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace MiniJob.Web.Pages;

/* Inherit your PageModel classes from this class.
 */
public abstract class MiniJobPageModel : AbpPageModel
{
    protected MiniJobPageModel()
    {
        LocalizationResourceType = typeof(MiniJobResource);
    }
}