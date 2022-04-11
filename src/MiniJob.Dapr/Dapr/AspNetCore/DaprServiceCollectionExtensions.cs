using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using MiniJob.Dapr;
using MiniJob.Dapr.AspNetCore;

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

    private static IDaprBuilder AddCoreServices(IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.TryAddSingleton<IDaprSidecarHost, DaprSidecarHost>();
        services.TryAddSingleton<DaprHttpContextHttpClientFactory>();
        services.TryAddSingleton<IDaprProcessHttpClientFactory>(x => x.GetRequiredService<DaprHttpContextHttpClientFactory>());
        services.TryAddSingleton<IDaprSidecarHttpClientFactory>(x => x.GetRequiredService<DaprHttpContextHttpClientFactory>());
        services.TryAddSingleton<IDaprApiTokenProvider, RandomStringApiTokenProvider>();
        services.TryAddSingleton<DaprApiTokenManager>();
        services.TryAddSingleton<IDaprApiTokenAccessor>(x => x.GetRequiredService<DaprApiTokenManager>());
        services.TryAddSingleton<IDaprApiTokenManager>(x => x.GetRequiredService<DaprApiTokenManager>());
        // If the service collection does not already contain a DaprSidecarHostedService implementation, don't try to add another one
        services.TryAddHostedService<DaprSidecarHostedService>();

        // Add the health checks and metrics
        services.AddHealthChecks().AddDaprSidecar();
        services.AddSingleton<IPrometheusCollector, DaprSidecarMetricsCollector>();
        services.AddSingleton<IPrometheusMetricFilter, DaprSidecarMetricFilter>();
        services.TryAddSingleton<IDaprMetricsCollectorRegistry, PrometheusCollectorRegistry>();

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
