using Volo.Abp.DependencyInjection;

namespace MiniJob.Dapr.Processes;

public class DaprProcessFactory : IDaprProcessFactory, ISingletonDependency
{
    public IDaprSidecarProcess CreateDaprSidecarProcess() => new DaprSidecarProcess();

    public IDaprPlacementProcess CreateDaprPlacementProcess() => new DaprPlacementProcess();

    public IDaprSentryProcess CreateDaprSentryProcess() => new DaprSentryProcess();
}
