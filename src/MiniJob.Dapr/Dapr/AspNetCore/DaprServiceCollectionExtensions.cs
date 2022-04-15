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
        // Only add the service if an existing implementation does not exist
        if (!services.HasAssignableService<IHostedService, TImplementation>())
        {
            services.AddHostedService<TImplementation>();
        }

        return services;
    }

    /// <summary>
    /// Adds the Dapr Sidecar process to the service container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureAction">An optional action to configure the component.</param>
    /// <returns>A <see cref="IDaprSidekickBuilder"/> for further configuration.</returns>
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
    /// Adds the Dapr Sidecar process to the service container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">A <see cref="IConfiguration"/> instance for configuring the component.</param>
    /// <param name="postConfigureAction">An optional action to configure the component after initial configuration is applied.</param>
    /// <returns>A <see cref="IDaprSidekickBuilder"/> for further configuration.</returns>
    public static IDaprBuilder AddMiniJobDapr(this IServiceCollection services, IConfiguration configuration, Action<DaprOptions> postConfigureAction = null)
    {
        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        // Try to get primary section "MiniJobDapr" first
        // Otherwise fall back to section "Dapr"
        var sectionName = configuration.GetSection(DaprOptions.SectionName).Exists() ? DaprOptions.SectionName : "Dapr";
        return AddMiniJobDapr(services, sectionName, configuration, postConfigureAction);
    }

    /// <summary>
    /// Adds the Dapr Sidecar process to the service container using the configuration section specified by <paramref name="name"/>.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="name">The name of the configuration section containing the settings in <paramref name="configuration"/>.</param>
    /// <param name="configuration">A <see cref="IConfiguration"/> instance for configuring the component.</param>
    /// <param name="postConfigureAction">An optional action to configure the component after initial configuration is applied.</param>
    /// <returns>A <see cref="IDaprSidekickBuilder"/> for further configuration.</returns>
    public static IDaprBuilder AddMiniJobDapr(this IServiceCollection services, string name, IConfiguration configuration, Action<DaprOptions> postConfigureAction = null)
    {
        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        var builder = AddCoreServices(services);

        // Create a new configuration based on the initial configuration but with the additional
        // support for setting/overriding top-level properties using environment variables.
        var daprConfig = new ConfigurationBuilder()
            .AddConfiguration(configuration.GetSection(name))
            .AddEnvironmentVariables(DaprOptions.EnvironmentVariablePrefix)
            .Build();
        services.Configure<DaprOptions>(daprConfig);

        if (postConfigureAction != null)
        {
            services.PostConfigure(postConfigureAction);
        }

        // The Dapr Sidecar can take 5 seconds to shutdown, which is the default shutdown time for IHostedService.
        // So set the default shutdown timeout to 10 seconds. This can be overridden in configuration using HostOptions.
        // See https://andrewlock.net/extending-the-shutdown-timeout-setting-to-ensure-graceful-ihostedservice-shutdown/
        services.Configure<HostOptions>(opts => opts.ShutdownTimeout = System.TimeSpan.FromSeconds(10));

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
        // Check to see if any existing service of type TImplementation (or a subclass) has been registered.
        foreach (var service in services)
        {
            if (service.ServiceType != typeof(TService))
            {
                continue;
            }

            if (typeof(TImplementation).IsAssignableFrom(service.ImplementationType))
            {
                // Already assigned
                return true;
            }
        }

        return false;
    }
}
