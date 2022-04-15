using Dapr.Actors.Runtime;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
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
        //context.Services.AddConventionalRegistrar(new ActorConventionalRegistrar());

        var configuration = context.Services.GetConfiguration();
        Configure<MiniJobDaprOptions>(configuration.GetSection(MiniJobDaprOptions.Dapr));

        var options = new MiniJobDaprOptions();
        context.Services.GetSingletonInstanceOrNull<IConfigureOptions<MiniJobDaprOptions>>()
            .Configure(options);

        // 配置Dapr环境变量
        Environment.SetEnvironmentVariable("DAPR_HTTP_PORT", options.DaprHttpPort.ToString());
        Environment.SetEnvironmentVariable("DAPR_GRPC_PORT", options.DaprGrpcPort.ToString());
        if (!options.DaprApiToken.IsNullOrWhiteSpace())
        {
            Environment.SetEnvironmentVariable("DAPR_API_TOKEN", options.DaprApiToken);
        }

        context.Services.PreConfigure<IMvcBuilder>(builder =>
        {
            builder.AddDapr();
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.TryAddSingleton<ActorActivatorFactory, MiniJobActorActivatorFactory>();
        //context.Services.AddActors(options => { });

        var configuration = context.Services.GetConfiguration();
        context.Services.AddMiniJobDapr(configuration);

        //Configure<AbpEndpointRouterOptions>(options =>
        //{
        //    options.EndpointConfigureActions.Add(endpointContext =>
        //    {
        //        endpointContext.Endpoints.MapActorsHandlers();
        //    });
        //});
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();

        if (app != null)
        {
            // 应用完全启动后运行Dapr Sidecar
            var lifeTime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();
            lifeTime.ApplicationStarted.Register(() =>
            {
                var options = app.ApplicationServices.GetRequiredService<IOptions<MiniJobDaprOptions>>().Value;
                var addressFeature = app.ServerFeatures.Get<IServerAddressesFeature>();

                if (addressFeature != null)
                {
                    options.RunDaprSidecar(addressFeature.Addresses.FirstOrDefault());
                }
            });
        }
    }
}