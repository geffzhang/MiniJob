using Microsoft.AspNetCore.Mvc;
using MiniJob.Jobs;
using System.Threading.Tasks;

namespace MiniJob.Web.Pages.Jobs.AppInfos
{
    public class CreateModalModel : MiniJobPageModel
    {
        [BindProperty]
        public CreateUpdateAppInfoDto AppInfo { get; set; }

        private readonly IAppInfoAppService _appInfoAppService;

        public CreateModalModel(IAppInfoAppService appInfoAppService)
        {
            _appInfoAppService = appInfoAppService;
        }

        public void OnGet()
        {
            AppInfo = new CreateUpdateAppInfoDto();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _appInfoAppService.CreateAsync(AppInfo);
            return NoContent();
        }
    }
}
