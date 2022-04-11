using MiniJob.Dapr.Options;

namespace MiniJob.Dapr.Processes;

public interface IDaprProcess<TOptions>
    where TOptions : DaprProcessOptions
{
    /// <summary>
    /// Gets the options that were actually used to successfully start the process the last time it was launched.
    /// This will include all calculated file paths and auto-assigned ports.
    /// </summary>
    TOptions LastSuccessfulOptions { get; }

    DaprProcessInfo GetProcessInfo();

    bool Start(Func<DaprOptions> optionsAccessor, CancellationToken cancellationToken = default);

    void Stop(CancellationToken cancellationToken = default);

    event EventHandler<DaprProcessStartingEventArgs<TOptions>> Starting;

    event EventHandler<DaprProcessStoppingEventArgs> Stopping;
}
