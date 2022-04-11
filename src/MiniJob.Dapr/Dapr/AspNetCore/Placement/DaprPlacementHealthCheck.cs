namespace MiniJob.Dapr.AspNetCore.Placement;

public class DaprPlacementHealthCheck : DaprProcessHealthCheck
{
    public DaprPlacementHealthCheck(
        IDaprPlacementHost daprPlacementHost)
        : base(daprPlacementHost)
    {
    }
}
