using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MiniJob.Dapr.Options;
using MiniJob.Dapr.Processes;

namespace MiniJob.Dapr.AspNetCore;

public abstract class DaprHostedService<TProcessHost, TProcessOptions> : IHostedService
        where TProcessHost : IDaprProcessHost
        where TProcessOptions : DaprProcessOptions
{
    private readonly TProcessHost _daprProcessHost;
    private readonly IOptionsMonitor<DaprOptions> _optionsAccessor;

    protected DaprHostedService(
        TProcessHost daprProcessHost,
        IOptionsMonitor<DaprOptions> optionsAccessor)
    {
        _daprProcessHost = daprProcessHost;
        _optionsAccessor = optionsAccessor;
    }

    public virtual Task StartAsync(CancellationToken cancellationToken)
    {
        _daprProcessHost.Start(
            () =>
            {
                var options = _optionsAccessor.CurrentValue;
                OnStarting(options, cancellationToken);
                return options;
            }, cancellationToken);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.Run(() => _daprProcessHost.Stop(cancellationToken), cancellationToken);
    }

    protected virtual void OnStarting(DaprOptions options, CancellationToken cancellationToken)
    {
    }
}
