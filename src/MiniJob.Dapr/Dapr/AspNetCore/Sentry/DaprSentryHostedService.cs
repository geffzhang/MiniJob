﻿using Microsoft.Extensions.Options;
using MiniJob.Dapr.AspNetCore.Metrics;

namespace MiniJob.Dapr.AspNetCore.Sentry;

public class DaprSentryHostedService : DaprHostedService<IDaprSentryHost, DaprSentryOptions>
{
    public DaprSentryHostedService(
        IDaprSentryHost daprSentryHost,
        IOptionsMonitor<DaprOptions> optionsAccessor)
        : base(daprSentryHost, optionsAccessor)
    {
    }

    protected override void OnStarting(DaprOptions options, CancellationToken cancellationToken)
    {
        // Assign metrics
        options.Sentry ??= new DaprSentryOptions();
        options.Sentry.Metrics ??= new DaprMetricsOptions();
        options.Sentry.Metrics.SetLabel(DaprMetricsConstants.ServiceLabelName, options.Sidecar?.AppId);
        options.Sentry.Metrics.SetLabel(DaprMetricsConstants.AppLabelName, DaprMetricsConstants.DaprSentryLabel);
    }
}
