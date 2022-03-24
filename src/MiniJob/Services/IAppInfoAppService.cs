using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace MiniJob.Jobs;

public interface IAppInfoAppService :
    ICrudAppService< //Defines CRUD methods
        AppInfoDto, //Used to show
        Guid, //Primary key
        PagedAndSortedResultRequestDto, //Used for paging/sorting
        CreateUpdateAppInfoDto> //Used to create/update
{
}