namespace MiniJob.Dapr.AspNetCore.Sidecar;

public class DaprSidecarHealthCheck : DaprProcessHealthCheck
{
    public DaprSidecarHealthCheck(
        IDaprSidecarHost daprSidecarHost)
        : base(daprSidecarHost)
    {
    }
}
