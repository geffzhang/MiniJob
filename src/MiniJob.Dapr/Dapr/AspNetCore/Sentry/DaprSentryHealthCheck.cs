﻿namespace MiniJob.Dapr.AspNetCore.Sentry;

public class DaprSentryHealthCheck : DaprProcessHealthCheck
{
    public DaprSentryHealthCheck(
        IDaprSentryHost daprSentryHost)
        : base(daprSentryHost)
    {
    }
}
