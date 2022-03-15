using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MiniJob.Dapr;
using MiniJob.Dapr.Actors;
using MiniJob.MultiTenancy;
using MiniJob.Scheduler;
using Volo.Abp;
using Volo.Abp.AuditLogging;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Emailing;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.IdentityServer;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement.Identity;
using Volo.Abp.PermissionManagement.IdentityServer;
using Volo.Abp.Reflection;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;
using Volo.Abp.Threading;

namespace MiniJob;

[DependsOn(
    typeof(MiniJobDomainSharedModule),
    typeof(AbpAuditLoggingDomainModule),
    typeof(AbpBackgroundJobsDomainModule),
    typeof(AbpFeatureManagementDomainModule),
    typeof(AbpIdentityDomainModule),
    typeof(AbpPermissionManagementDomainIdentityModule),
    typeof(AbpIdentityServerDomainModule),
    typeof(AbpPermissionManagementDomainIdentityServerModule),
    typeof(AbpSettingManagementDomainModule),
    typeof(AbpTenantManagementDomainModule),
    typeof(AbpEmailingModule),
    typeof(MiniJobDaprModule)
)]
public class MiniJobDomainModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        AutoAddSchedulers(context.Services);
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpMultiTenancyOptions>(options =>
        {
            options.IsEnabled = MultiTenancyConsts.IsEnabled;
        });

#if DEBUG
        context.Services.Replace(ServiceDescriptor.Singleton<IEmailSender, NullEmailSender>());
#endif
    }

    public override void OnPreApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();

        if (app != null)
        {
            // 应用完全启动后启动所有注册的调度器
            // 要先启动Sidecar才能启动调度器，而ApplicationStarted的注册顺序与执行顺序相反，
            // 所以启动调度器方法要先注册，故应放在OnPreApplicationInitialization方法中注册
            var lifeTime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();
            lifeTime.ApplicationStarted.Register(() =>
            {
                // 等Sidecar启动
                Thread.Sleep(10000);
                var options = app.ApplicationServices.GetRequiredService<IOptions<MiniJobOptions>>().Value;
                foreach (var type in options.Schedulers)
                {
                    var scheduler = (IScheduler)ActorHelper.CreateDefaultActor(type);
                    AsyncHelper.RunSync(() => scheduler.StartAsync());
                }
            });
        }
    }

    public override async Task OnApplicationShutdownAsync(ApplicationShutdownContext context)
    {
        var options = context.ServiceProvider.GetRequiredService<IOptions<MiniJobOptions>>().Value;
        foreach (var type in options.Schedulers)
        {
            var scheduler = (IScheduler)ActorHelper.CreateDefaultActor(type);
            await scheduler.StopAsync();
        }
    }

    private static void AutoAddSchedulers(IServiceCollection services)
    {
        var schedulerTypes = new List<Type>();

        services.OnRegistred(context =>
        {
            if (typeof(IScheduler).IsAssignableFrom(context.ImplementationType))
            {
                schedulerTypes.AddIfNotContains(context.ImplementationType);
            }
        });

        services.Configure<MiniJobOptions>(options =>
        {
            options.Schedulers.AddIfNotContains(schedulerTypes);
        });
    }

    private static void RegisterJobs(IServiceCollection services)
    {
        var jobTypes = new List<Type>();

        services.OnRegistred(context =>
        {
            if (ReflectionHelper.IsAssignableToGenericType(context.ImplementationType, typeof(IBackgroundJob<>)) ||
                ReflectionHelper.IsAssignableToGenericType(context.ImplementationType, typeof(IAsyncBackgroundJob<>)))
            {
                jobTypes.Add(context.ImplementationType);
            }
        });

        services.Configure<AbpBackgroundJobOptions>(options =>
        {
            foreach (var jobType in jobTypes)
            {
                options.AddJob(jobType);
            }
        });
    }
}