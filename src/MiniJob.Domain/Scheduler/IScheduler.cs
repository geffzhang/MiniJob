using Dapr.Actors;
using System.Threading;
using System.Threading.Tasks;

namespace MiniJob.Scheduler
{
    public interface IScheduler : IActor
    {
        Task StartAsync(CancellationToken cancellationToken = default);

        Task StopAsync(CancellationToken cancellationToken = default);
    }
}
