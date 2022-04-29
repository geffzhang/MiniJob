using Dapr.Client;
using Microsoft.Extensions.Logging;
using MiniJob.Dapr.Processes;

namespace MiniJob.Dapr.AspNetCore.Sentry;

public class DaprSentryHost : DaprProcessHost<DaprSentryOptions>, IDaprSentryHost
{
    public DaprSentryHost(
        IDaprSentryProcess daprSentryProcess,
        DaprClient daprClient,
        ILoggerFactory loggerFactory)
        : base(daprSentryProcess,
            daprClient,
            loggerFactory.CreateLogger<DaprSentryHost>())
    {
    }
}
