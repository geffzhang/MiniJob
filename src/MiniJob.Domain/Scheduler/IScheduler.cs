using Dapr.Actors;

namespace MiniJob.Scheduler;

public interface IScheduler : IActor
{
    Task StartAsync(CancellationToken cancellationToken = default);

    Task StopAsync(CancellationToken cancellationToken = default);
}