using Dapr.Client;
using Microsoft.Extensions.Logging;
using MiniJob.Dapr.Processes;
using MiniJob.Dapr.Security;
using Volo.Abp.DependencyInjection;

namespace MiniJob.Dapr.AspNetCore.Sidecar;

public class DaprSidecarHost : DaprProcessHost<DaprSidecarOptions>, IDaprSidecarHost, ISingletonDependency
{
    private readonly IDaprApiTokenManager _daprApiTokenManager;
    private readonly IEnumerable<IDaprSidecarProcessInterceptor> _daprSidecarInterceptors;

    public DaprSidecarHost(
        IDaprProcessFactory daprProcessFactory,
        DaprClient daprHttpClientFactory,
        IDaprApiTokenManager daprApiTokenManager,
        ILoggerFactory loggerFactory,
        IEnumerable<IDaprSidecarProcessInterceptor> daprSidecarInterceptors = null)
        : base(() =>
            daprProcessFactory.CreateDaprSidecarProcess(),
            daprHttpClientFactory,
            loggerFactory.CreateLogger<DaprSidecarHost>())
    {
        _daprApiTokenManager = daprApiTokenManager;
        _daprSidecarInterceptors = daprSidecarInterceptors;
    }

    protected override void OnProcessStarting(DaprProcessStartingEventArgs<DaprSidecarOptions> args)
    {
        var options = args.Options;

        // Invoke any interceptors
        InvokeInterceptors(interceptor => interceptor.OnStarting(options));

        // Update the API tokens - this can happen here because Environment Variables are set after this method returns
        // If the API token is not defined in configuration then replace it with the default API token if required.
        if (string.IsNullOrEmpty(options.DaprApiToken) && options.UseDefaultDaprApiToken == true)
        {
            // Use the default Dapr API token
            options.DaprApiToken = _daprApiTokenManager.DaprApiToken;
        }
        else
        {
            // Use the specified Dapr API token
            _daprApiTokenManager.SetDaprApiToken(options.DaprApiToken);
        }

        if (string.IsNullOrEmpty(options.AppApiToken) && options.UseDefaultAppApiToken == true)
        {
            // Use the default App API token
            options.AppApiToken = _daprApiTokenManager.AppApiToken;
        }
        else
        {
            // Use the specified App API token
            _daprApiTokenManager.SetAppApiToken(options.AppApiToken);
        }
    }

    private void InvokeInterceptors(Action<IDaprSidecarProcessInterceptor> invoke)
    {
        if (_daprSidecarInterceptors?.Any() != true)
        {
            return;
        }

        // Invoke each interceptor
        foreach (var interceptor in _daprSidecarInterceptors)
        {
            invoke(interceptor);
        }
    }
}
