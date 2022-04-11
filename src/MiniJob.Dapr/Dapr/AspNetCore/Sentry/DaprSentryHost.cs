using Dapr.Client;
using Microsoft.Extensions.Logging;
using MiniJob.Dapr.Processes;

namespace MiniJob.Dapr.AspNetCore.Sentry;

public class DaprSentryHost : DaprProcessHost<DaprSentryOptions>, IDaprSentryHost
{
    public DaprSentryHost(
        IDaprProcessFactory daprProcessFactory,
        DaprClient daprHttpClientFactory,
        ILoggerFactory loggerFactory)
        : base(() =>
            daprProcessFactory.CreateDaprSentryProcess(),
            daprHttpClientFactory,
            loggerFactory.CreateLogger<DaprSentryHost>())
    {
    }
}
