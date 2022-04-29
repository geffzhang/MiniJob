using Dapr.Client;
using Microsoft.Extensions.Logging;
using MiniJob.Dapr.Processes;

namespace MiniJob.Dapr.AspNetCore.Placement;

public sealed class DaprPlacementHost : DaprProcessHost<DaprPlacementOptions>, IDaprPlacementHost
{
    public DaprPlacementHost(
        IDaprPlacementProcess daprPlacementProcess,
        DaprClient daprClient,
        ILoggerFactory loggerFactory)
        : base(daprPlacementProcess,
            daprClient,
            loggerFactory.CreateLogger<DaprPlacementHost>())
    {
    }
}
