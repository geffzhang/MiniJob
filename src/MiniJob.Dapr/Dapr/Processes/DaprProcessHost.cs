using Microsoft.Extensions.Logging;
using MiniJob.Dapr.Http;

namespace MiniJob.Dapr.Processes;

public abstract class DaprProcessHost<TOptions> : IDaprProcessHost
        where TOptions : Options.DaprProcessOptions
{
    private readonly object _processLock = new object();
    private readonly Func<IDaprProcess<TOptions>> _createDaprProcess;
    private readonly ILogger _logger;

    protected DaprProcessHost(
        Func<IDaprProcess<TOptions>> createDaprProcess,
        IDaprProcessHttpClientFactory daprHttpClientFactory,
        ILogger logger)
    {
        _createDaprProcess = createDaprProcess ?? throw new ArgumentNullException(nameof(createDaprProcess));
        DaprHttpClientFactory = daprHttpClientFactory ?? throw new ArgumentNullException(nameof(daprHttpClientFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // Internal for testing
    protected internal IDaprProcessHttpClientFactory DaprHttpClientFactory { get; }

    public DaprProcessInfo GetProcessInfo() => Process?.GetProcessInfo() ?? DaprProcessInfo.Unknown;

    public TOptions GetProcessOptions() => Process?.LastSuccessfulOptions;

    Options.DaprProcessOptions IDaprProcessHost.GetProcessOptions() => GetProcessOptions();

    public bool Start(Func<DaprOptions> optionsAccessor, CancellationToken cancellationToken)
    {
        lock (_processLock)
        {
            // Stop process if already running
            Stop(cancellationToken);

            // Start the new process.
            Process = _createDaprProcess();
            Process.Starting += ProcessStarting;
            Process.Stopping += ProcessStopping;
            return Process.Start(optionsAccessor, cancellationToken);
        }
    }

    public void Stop(CancellationToken cancellationToken)
    {
        lock (_processLock)
        {
            if (Process != null)
            {
                // Stop the process
                Process.Stop(cancellationToken);
                Process.Starting -= ProcessStarting;
                Process.Stopping -= ProcessStopping;

                // Clear out the variables
                Process = null;
            }
        }
    }

    public async Task<DaprHealthResult> GetHealthAsync(CancellationToken cancellationToken)
    {
        var client = DaprHttpClientFactory.CreateDaprHttpClient();
        var uri = Process?.LastSuccessfulOptions?.GetHealthUri();
        if (uri == null || client == null)
        {
            return DaprHealthResult.Unknown;
        }

        // Check the endpoint - this will throw an exception if an error occurs.
        var result = await client.GetAsync(uri, cancellationToken);
        return new DaprHealthResult(result.StatusCode, result.ReasonPhrase);
    }

    public async Task<int> WriteMetricsAsync(Stream stream, CancellationToken cancellationToken)
    {
        var client = DaprHttpClientFactory.CreateDaprHttpClient();
        var uri = Process?.LastSuccessfulOptions?.GetMetricsUri();
        if (uri == null || client == null)
        {
            return 0;
        }

        try
        {
            var response = await client.GetAsync(uri, cancellationToken);
            response.EnsureSuccessStatusCode();
            var bytes = await response.Content.ReadAsByteArrayAsync();
            stream.Write(bytes, 0, bytes.Length);
            return bytes.Length;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while obtaining the Dapr process metrics from {DaprProcessMetricsUri}", uri);
            return 0;
        }
    }

    // Internal for testing
    protected internal IDaprProcess<TOptions> Process { get; private set; }

    protected virtual void OnProcessStarting(DaprProcessStartingEventArgs<TOptions> args)
    {
    }

    protected virtual void OnProcessStopping(DaprProcessStoppingEventArgs args)
    {
    }

    private void ProcessStarting(object sender, DaprProcessStartingEventArgs<TOptions> e) => OnProcessStarting(e);

    private void ProcessStopping(object sender, DaprProcessStoppingEventArgs e) => OnProcessStopping(e);
}
