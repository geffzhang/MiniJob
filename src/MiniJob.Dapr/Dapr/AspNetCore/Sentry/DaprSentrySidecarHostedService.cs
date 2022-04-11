using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MiniJob.Dapr.AspNetCore.Sidecar;
using MiniJob.Dapr.Processes;

namespace MiniJob.Dapr.AspNetCore.Sentry;

/// <summary>
/// 用于管理 sidecar 生命周期的托管服务
/// <para>在启动 sidecar 之前，等待 Sentry 服务成功启动并分配端口/环境变量</para>
/// </summary>
public class DaprSentrySidecarHostedService : DaprSidecarHostedService
{
    private readonly IDaprSentryHost _daprSentryHost;
    private readonly ILogger<DaprSidecarHostedService> _logger;

    public DaprSentrySidecarHostedService(
        IDaprSidecarHost daprSidecarHost,
        IDaprSentryHost daprSentryHost,
        IOptionsMonitor<DaprOptions> optionsAccessor,
        ILogger<DaprSentrySidecarHostedService> logger,
        IServiceProvider serviceProvider = null)
        : base(daprSidecarHost, optionsAccessor, serviceProvider)
    {
        _daprSentryHost = daprSentryHost;
        _logger = logger;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await Task.Run(
            async () =>
            {
                var processInfo = _daprSentryHost.GetProcessInfo();
                while (!processInfo.IsRunning && processInfo.Status != DaprProcessStatus.Disabled)
                {
                    _logger.LogInformation("Dapr Sidecar process is waiting for the Dapr Sentry process to finish starting up...");
                    await Task.Delay(250);
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }

                    processInfo = _daprSentryHost.GetProcessInfo();
                }
            }, cancellationToken)
            .ContinueWith(_ => base.StartAsync(cancellationToken));
    }
}
