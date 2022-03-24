using MiniJob.Localization;
using MiniJob.Permissions;
using Volo.Abp.Identity.Web.Navigation;
using Volo.Abp.SettingManagement.Web.Navigation;
using Volo.Abp.TenantManagement.Web.Navigation;
using Volo.Abp.UI.Navigation;

namespace MiniJob.Menus;

public class MiniJobMenuContributor : IMenuContributor
{
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
    }

    private async Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var administration = context.Menu.GetAdministration();
        var l = context.GetLocalizer<MiniJobResource>();

        context.Menu.Items.Insert(
            0,
            new ApplicationMenuItem(
                MiniJobMenus.Home,
                l["Menu:Home"],
                "~/",
                icon: "fas fa-home",
                order: 0
            )
        );

        if (MiniJobConsts.IsMultiTenant)
        {
            administration.SetSubItemOrder(TenantManagementMenuNames.GroupName, 1);
        }
        else
        {
            administration.TryRemoveMenuItem(TenantManagementMenuNames.GroupName);
        }

        administration.SetSubItemOrder(IdentityMenuNames.GroupName, 2);
        administration.SetSubItemOrder(SettingManagementMenuNames.GroupName, 3);

        var miniJobMenu = new ApplicationMenuItem(
            "MiniJob",
            l["Menu:MiniJob"],
            icon: "fa fa-clock"
        );

        context.Menu.AddItem(miniJobMenu);

        if (await context.IsGrantedAsync(MiniJobPermissions.AppInfos.Default))
        {
            miniJobMenu.AddItem(new ApplicationMenuItem(
                "MiniJob.AppInfos",
                l["Menu:AppInfos"],
                url: "/Jobs/AppInfos"
            ));
        }

        if (await context.IsGrantedAsync(MiniJobPermissions.JobInfos.Default))
        {
            miniJobMenu.AddItem(new ApplicationMenuItem(
                "MiniJob.JobInfos",
                l["Menu:JobInfos"],
                url: "/Jobs/JobInfos"
            ));
        }
    }
}
