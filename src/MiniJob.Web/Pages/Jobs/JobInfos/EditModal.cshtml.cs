using Microsoft.AspNetCore.Mvc;
using MiniJob.Jobs;
using System;
using System.Threading.Tasks;

namespace MiniJob.Web.Pages.Jobs.JobInfos
{
    public class EditModalModel : MiniJobPageModel
    {
        [HiddenInput]
        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        [BindProperty]
        public CreateUpdateJobInfoDto JobInfo { get; set; }

        private readonly IJobInfoAppService _jobInfoAppService;

        public EditModalModel(IJobInfoAppService jobInfoAppService)
        {
            _jobInfoAppService = jobInfoAppService;
        }

        public async Task OnGetAsync()
        {
            var jobInfoDto = await _jobInfoAppService.GetAsync(Id);
            JobInfo = ObjectMapper.Map<JobInfoDto, CreateUpdateJobInfoDto>(jobInfoDto);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _jobInfoAppService.UpdateAsync(Id, JobInfo);
            return NoContent();
        }
    }
}
