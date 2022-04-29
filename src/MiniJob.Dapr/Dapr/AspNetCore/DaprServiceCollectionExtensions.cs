using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MiniJob.Dapr;
using MiniJob.Dapr.AspNetCore;
using MiniJob.Dapr.AspNetCore.Sidecar;

namespace Microsoft.Extensions.DependencyInjection;

public static class DaprServiceCollectionExtensions
{
    public static IServiceCollection TryAddHostedService<TImplementation>(this IServiceCollection services)
        where TImplementation : class, IHostedService
    {
        // 只有在现有实现不存在的情况下才添加服务
        if (!services.HasAssignableService<IHostedService, TImplementation>())
        {
            services.AddHostedService<TImplementation>();
        }

        return services;
    }

    /// <summary>
    /// 将Dapr Sidecar进程添加到服务容器
    /// </summary>
    /// <param name="services">服务容器</param>
    /// <param name="configureAction">配置组件的可选操作</param>
    /// <returns><see cref="IDaprBuilder"/> 用于进一步配置</returns>
    public static IDaprBuilder AddMiniJobDapr(this IServiceCollection services, Action<DaprOptions> configureAction = null)
    {
        var builder = AddCoreServices(services);

        // Configure the options
        if (configureAction != null)
        {
            services.AddOptions<DaprOptions>().Configure(configureAction);
        }
        else
        {
            services.AddOptions<DaprOptions>();
        }

        return builder;
    }

    /// <summary>
    /// 将Dapr Sidecar进程添加到服务容器
    /// </summary>
    /// <param name="services">服务容器</param>
    /// <param name="configuration">配置组件的 <see cref="IConfiguration"/> 实例</param>
    /// <param name="postConfigureAction">应用初始配置后配置组件的可选操作</param>
    /// <returns><see cref="IDaprBuilder"/> 用于进一步配置</returns>
    public static IDaprBuilder AddMiniJobDapr(this IServiceCollection services, IConfiguration configuration, Action<DaprOptions> postConfigureAction = null)
    {
        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        // 首先尝试获取 "MiniJobDapr" 配置节，如果不存在则回退到 "Dapr" 配置节
        var sectionName = configuration.GetSection(DaprOptions.SectionName).Exists() ? DaprOptions.SectionName : "Dapr";
        return AddMiniJobDapr(services, sectionName, configuration, postConfigureAction);
    }

    /// <summary>
    /// 使用 <paramref name="name"/> 指定的配置节将Dapr Sidecar进程添加到服务容器
    /// </summary>
    /// <param name="services">服务容器</param>
    /// <param name="name">配置节名称</param>
    /// <param name="configuration">配置组件的 <see cref="IConfiguration"/> 实例</param>
    /// <param name="postConfigureAction">应用初始配置后配置组件的可选操作</param>
    /// <returns><see cref="IDaprBuilder"/> 用于进一步配置</returns>
    public static IDaprBuilder AddMiniJobDapr(this IServiceCollection services, string name, IConfiguration configuration, Action<DaprOptions> postConfigureAction = null)
    {
        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        var builder = AddCoreServices(services);

        // 在初始配置的基础上创建一个新的配置，但要支持使用环境变量设置/覆盖顶级属性
        var daprConfig = new ConfigurationBuilder()
            .AddConfiguration(configuration.GetSection(name))
            .AddEnvironmentVariables(DaprOptions.EnvironmentVariablePrefix)
            .Build();
        services.Configure<DaprOptions>(daprConfig);

        if (postConfigureAction != null)
        {
            services.PostConfigure(postConfigureAction);
        }

        // Dapr Sidecar需要5秒才能关闭，这是 IHostedService 的默认关闭时间
        // 因此，将默认关机超时设置为10秒。这可以在配置中使用HostOptions覆盖
        // See https://andrewlock.net/extending-the-shutdown-timeout-setting-to-ensure-graceful-ihostedservice-shutdown/
        services.Configure<HostOptions>(opts => opts.ShutdownTimeout = TimeSpan.FromSeconds(10));

        return builder;
    }

    private static IDaprBuilder AddCoreServices(IServiceCollection services)
    {
        // If the service collection does not already contain a DaprSidecarHostedService implementation, don't try to add another one
        services.TryAddHostedService<DaprSidecarHostedService>();

        // Add the health checks and metrics
        services.AddHealthChecks().AddDaprSidecar();

        // Return the builder
        return new DaprBuilder(services);
    }

    public static bool HasAssignableService<TService, TImplementation>(this IServiceCollection services)
    {
        // 检查是否已经注册了TImplementation类型(或子类)的现有服务
        foreach (var service in services)
        {
            if (service.ServiceType != typeof(TService))
            {
                continue;
            }

            if (typeof(TImplementation).IsAssignableFrom(service.ImplementationType))
            {
                // 已经分配
                return true;
            }
        }

        return false;
    }
}
