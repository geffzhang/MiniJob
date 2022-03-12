using Microsoft.AspNetCore.Mvc;
using MiniJob.Jobs;
using System.Threading.Tasks;

namespace MiniJob.Web.Pages.Jobs.JobInfos
{
    public class CreateModalModel : MiniJobPageModel
    {
        [BindProperty]
        public CreateUpdateJobInfoDto JobInfo { get; set; }

        private readonly IJobInfoAppService _jobInfoAppService;

        public CreateModalModel(IJobInfoAppService jobInfoAppService)
        {
            _jobInfoAppService = jobInfoAppService;
        }

        public void OnGet()
        {
            JobInfo = new CreateUpdateJobInfoDto();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _jobInfoAppService.CreateAsync(JobInfo);
            return NoContent();
        }
    }
}
