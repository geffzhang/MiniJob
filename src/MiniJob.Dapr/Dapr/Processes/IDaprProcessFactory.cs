namespace MiniJob.Dapr.Processes;

public interface IDaprProcessFactory
{
    IDaprSidecarProcess CreateDaprSidecarProcess();

    IDaprPlacementProcess CreateDaprPlacementProcess();

    IDaprSentryProcess CreateDaprSentryProcess();
}
