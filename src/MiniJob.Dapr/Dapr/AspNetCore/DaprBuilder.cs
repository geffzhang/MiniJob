using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using MiniJob.Dapr.AspNetCore.Placement;
using MiniJob.Dapr.AspNetCore.Sentry;
using MiniJob.Dapr.AspNetCore.Sidecar;

namespace MiniJob.Dapr.AspNetCore;

internal class DaprBuilder : IDaprBuilder
{
    private readonly IServiceCollection _services;

    internal DaprBuilder(IServiceCollection services)
    {
        _services = services;
    }

    public IDaprBuilder AddPlacement()
    {
        // Add the placement host
        _services.TryAddSingleton<IDaprPlacementHost, DaprPlacementHost>();
        _services.TryAddHostedService<DaprPlacementHostedService>();

        // Add the health checks and metrics
        _services.AddHealthChecks().AddDaprPlacement();

        // 将默认的sidecar托管服务重写为只有在Placement服务可用时才启动的服务
        ReplaceSidecarHostedService<DaprPlacementSidecarHostedService>();

        return this;
    }

    public IDaprBuilder AddSentry()
    {
        // Add the Sentry host
        _services.TryAddSingleton<IDaprSentryHost, DaprSentryHost>();
        _services.TryAddHostedService<DaprSentryHostedService>();

        // Add the health checks and metrics
        _services.AddHealthChecks().AddDaprSentry();

        // 将默认的sidecar托管服务重写为只有在Sentry服务可用时才启动的服务
        ReplaceSidecarHostedService<DaprSentrySidecarHostedService>();

        return this;
    }

    private void ReplaceSidecarHostedService<TImplementation>()
        where TImplementation : class, IHostedService
    {
        var sidecarHostedServiceDescriptor = _services.FirstOrDefault(x => x.ServiceType == typeof(IHostedService) && x.ImplementationType == typeof(DaprSidecarHostedService));
        if (sidecarHostedServiceDescriptor != null)
        {
            _services.Remove(sidecarHostedServiceDescriptor);
            _services.AddHostedService<TImplementation>();
        }
    }
}
