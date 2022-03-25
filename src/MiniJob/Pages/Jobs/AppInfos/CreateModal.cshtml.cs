using Microsoft.AspNetCore.Mvc;
using MiniJob.Jobs;

namespace MiniJob.Web.Pages.Jobs.AppInfos;

public class CreateModalModel : MiniJobPageModel
{
    [BindProperty]
    public CreateUpdateAppInfoDto AppInfo { get; set; }

    private readonly AppInfoAppService _appInfoAppService;

    public CreateModalModel(AppInfoAppService appInfoAppService)
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