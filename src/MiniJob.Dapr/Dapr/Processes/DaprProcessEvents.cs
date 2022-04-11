namespace MiniJob.Dapr.Processes;

public class DaprProcessStartingEventArgs<TOptions> : EventArgs
{
    public DaprProcessStartingEventArgs(TOptions options)
    {
        Options = options;
    }

    public TOptions Options { get; }
}

public class DaprProcessStoppingEventArgs : EventArgs
{
    public DaprProcessStoppingEventArgs(CancellationToken cancellationToken)
    {
        CancellationToken = cancellationToken;
    }

    public CancellationToken CancellationToken { get; }
}
