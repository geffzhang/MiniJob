using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace MiniJob.Jobs;

public interface IJobInfoAppService :
    ICrudAppService<
        JobInfoDto,
        Guid,
        PagedAndSortedResultRequestDto,
        CreateUpdateJobInfoDto>
{
}