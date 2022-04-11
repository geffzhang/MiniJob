namespace MiniJob.Dapr.Processes;

public interface IDaprSidecarProcessInterceptor
{
    void OnStarting(DaprSidecarOptions options);
}
