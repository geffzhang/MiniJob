using MiniJob.Entities.Jobs;
using MiniJob.Permissions;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace MiniJob.Jobs;

public class JobInfoAppService :
    CrudAppService<
        JobInfo,
        JobInfoDto,
        Guid,
        PagedAndSortedResultRequestDto,
        CreateUpdateJobInfoDto>,
    IJobInfoAppService
{
    public JobInfoAppService(IRepository<JobInfo, Guid> repository)
    : base(repository)
    {
        GetPolicyName = MiniJobPermissions.JobInfos.Default;
        GetListPolicyName = MiniJobPermissions.JobInfos.Default;
        CreatePolicyName = MiniJobPermissions.JobInfos.Create;
        UpdatePolicyName = MiniJobPermissions.JobInfos.Edit;
        DeletePolicyName = MiniJobPermissions.JobInfos.Delete;
    }
}