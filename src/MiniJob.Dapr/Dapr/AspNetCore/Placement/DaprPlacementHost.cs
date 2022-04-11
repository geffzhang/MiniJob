﻿using Dapr.Client;
using Microsoft.Extensions.Logging;
using MiniJob.Dapr.Processes;

namespace MiniJob.Dapr.AspNetCore.Placement;

public sealed class DaprPlacementHost : DaprProcessHost<DaprPlacementOptions>, IDaprPlacementHost
{
    public DaprPlacementHost(
        IDaprProcessFactory daprProcessFactory,
        DaprClient daprHttpClientFactory,
        ILoggerFactory loggerFactory)
        : base(() =>
            daprProcessFactory.CreateDaprPlacementProcess(),
            daprHttpClientFactory,
            loggerFactory.CreateLogger<DaprPlacementHost>())
    {
    }
}
