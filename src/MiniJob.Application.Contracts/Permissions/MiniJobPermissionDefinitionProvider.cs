using MiniJob.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace MiniJob.Permissions
{
    public class MiniJobPermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            var miniJobGroup = context.AddGroup(MiniJobPermissions.GroupName, L("Permission:MiniJob"));

            var appsPermission = miniJobGroup.AddPermission(MiniJobPermissions.AppInfos.Default, L("Permission:AppInfos"));
            appsPermission.AddChild(MiniJobPermissions.AppInfos.Create, L("Permission:AppInfos.Create"));
            appsPermission.AddChild(MiniJobPermissions.AppInfos.Edit, L("Permission:AppInfos.Edit"));
            appsPermission.AddChild(MiniJobPermissions.AppInfos.Delete, L("Permission:AppInfos.Delete"));

            var jobsPermission = miniJobGroup.AddPermission(MiniJobPermissions.JobInfos.Default, L("Permission:JobInfos"));
            jobsPermission.AddChild(MiniJobPermissions.JobInfos.Create, L("Permission:JobInfos.Create"));
            jobsPermission.AddChild(MiniJobPermissions.JobInfos.Edit, L("Permission:JobInfos.Edit"));
            jobsPermission.AddChild(MiniJobPermissions.JobInfos.Delete, L("Permission:JobInfos.Delete"));
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<MiniJobResource>(name);
        }
    }
}