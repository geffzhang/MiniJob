using Microsoft.AspNetCore.Mvc;
using MiniJob.Jobs;
using System;
using System.Threading.Tasks;

namespace MiniJob.Web.Pages.Jobs.AppInfos
{
    public class EditModalModel : MiniJobPageModel
    {
        [HiddenInput]
        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        [BindProperty]
        public CreateUpdateAppInfoDto AppInfo { get; set; }

        private readonly IAppInfoAppService _appInfoAppService;

        public EditModalModel(IAppInfoAppService appInfoAppService)
        {
            _appInfoAppService = appInfoAppService;
        }

        public async Task OnGetAsync()
        {
            var appInfoDto = await _appInfoAppService.GetAsync(Id);
            AppInfo = ObjectMapper.Map<AppInfoDto, CreateUpdateAppInfoDto>(appInfoDto);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _appInfoAppService.UpdateAsync(Id, AppInfo);
            return NoContent();
        }
    }
}
