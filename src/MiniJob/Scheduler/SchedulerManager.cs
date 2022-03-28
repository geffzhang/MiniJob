using Microsoft.Extensions.Options;
using MiniJob.Dapr.Actors;
using Polly;
using Volo.Abp.DependencyInjection;

namespace MiniJob.Scheduler;

public class SchedulerManager : ISchedulerManager, ISingletonDependency
{
    protected MiniJobSchedulerOptions SchedulerOptions { get; }

    public SchedulerManager(IOptions<MiniJobSchedulerOptions> options)
    {
        SchedulerOptions = options.Value;
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        foreach (var type in SchedulerOptions.Schedulers)
        {
            var scheduler = (IScheduler)ActorHelper.CreateDefaultActor(type);

            await Policy.Handle<Exception>()
                .WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(Math.Pow(2, i)))
                .ExecuteAsync(() => scheduler.StartAsync(cancellationToken));
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        foreach (var type in SchedulerOptions.Schedulers)
        {
            var scheduler = (IScheduler)ActorHelper.CreateDefaultActor(type);

            await scheduler.StopAsync(cancellationToken);
        }
    }
}
