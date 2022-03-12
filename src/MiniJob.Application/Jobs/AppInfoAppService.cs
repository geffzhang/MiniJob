using MiniJob.Permissions;
using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace MiniJob.Jobs
{
    public class AppInfoAppService :
        CrudAppService<
            AppInfo,
            AppInfoDto,
            Guid,
            PagedAndSortedResultRequestDto,
            CreateUpdateAppInfoDto>,
        IAppInfoAppService
    {
        public AppInfoAppService(IRepository<AppInfo, Guid> repository)
            : base(repository)
        {
            GetPolicyName = MiniJobPermissions.AppInfos.Default;
            GetListPolicyName = MiniJobPermissions.AppInfos.Default;
            CreatePolicyName = MiniJobPermissions.AppInfos.Create;
            UpdatePolicyName = MiniJobPermissions.AppInfos.Edit;
            DeletePolicyName = MiniJobPermissions.AppInfos.Delete;
        }
    }
}
