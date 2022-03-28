namespace MiniJob.Scheduler;

public interface ISchedulerManager
{
    Task StartAsync(CancellationToken cancellationToken = default);

    Task StopAsync(CancellationToken cancellationToken = default);
}
