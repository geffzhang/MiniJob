using Dapr.Actors;

namespace MiniJob.Scheduler;

public interface IJobTrackerActor : IActor
{
    Task TrackAsync();
}
