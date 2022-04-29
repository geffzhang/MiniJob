using Dapr.Actors.Runtime;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MiniJob.Dapr.Actors;
using Volo.Abp;
using Volo.Abp.AspNetCore;
using Volo.Abp.Autofac;
using Volo.Abp.Guids;
using Volo.Abp.Modularity;

namespace MiniJob.Dapr;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpGuidsModule),
    typeof(AbpAspNetCoreModule)
)]
public class MiniJobDaprModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        // 自动注册Actor
        context.Services.AddConventionalRegistrar(new ActorConventionalRegistrar());

        context.Services.PreConfigure<IMvcBuilder>(builder =>
        {
            builder.AddDapr();
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.TryAddSingleton<ActorActivatorFactory, MiniJobActorActivatorFactory>();
        context.Services.AddActors(options => { });

        var configuration = context.Services.GetConfiguration();
        context.Services.AddMiniJobDapr(configuration);

        Configure<AbpEndpointRouterOptions>(options =>
        {
            options.EndpointConfigureActions.Add(endpointContext =>
            {
                endpointContext.Endpoints.MapActorsHandlers();
            });
        });
    }
}