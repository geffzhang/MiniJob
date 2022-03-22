using Microsoft.Extensions.DependencyInjection;
using MiniJob.TimeWheel;
using Polly;
using System;
using Volo.Abp.Autofac;
using Volo.Abp.Http.Client;
using Volo.Abp.Http.Client.IdentityModel;
using Volo.Abp.Modularity;

namespace MiniJob.HttpApi.Client.ConsoleTestApp;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(MiniJobHttpApiClientModule),
    typeof(AbpHttpClientIdentityModelModule),
    typeof(MiniJobDomainModule)
    )]
public class MiniJobConsoleApiClientModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<AbpHttpClientBuilderOptions>(options =>
        {
            options.ProxyClientBuildActions.Add((remoteServiceName, clientBuilder) =>
            {
                clientBuilder.AddTransientHttpErrorPolicy(
                    policyBuilder => policyBuilder.WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(Math.Pow(2, i)))
                );
            });
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<TimingWheelOptions>(options =>
        {
            options.TickSpan = 1000;
        });
    }
}