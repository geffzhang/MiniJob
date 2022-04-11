namespace MiniJob.Dapr.Processes;

public interface IDaprProcessUpdater
{
    void UpdateStatus(DaprProcessStatus status);

    void UpdateVersion(string version);
}
