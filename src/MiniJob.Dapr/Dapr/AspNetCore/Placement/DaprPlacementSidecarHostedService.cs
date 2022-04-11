using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MiniJob.Dapr.AspNetCore.Sidecar;
using MiniJob.Dapr.Processes;

namespace MiniJob.Dapr.AspNetCore.Placement;

/// <summary>
/// 用于管理 sidecar 生命周期的托管服务
/// <para>在启动 sidecar 之前，等待 placement 服务成功启动并分配端口/环境变量</para>
/// </summary>
public class DaprPlacementSidecarHostedService : DaprSidecarHostedService
{
    private readonly IDaprPlacementHost _daprPlacementHost;
    private readonly ILogger<DaprSidecarHostedService> _logger;

    public DaprPlacementSidecarHostedService(
        IDaprSidecarHost daprSidecarHost,
        IDaprPlacementHost daprPlacementHost,
        IOptionsMonitor<DaprOptions> optionsAccessor,
        ILogger<DaprPlacementSidecarHostedService> logger,
        IServiceProvider serviceProvider = null)
        : base(daprSidecarHost, optionsAccessor, serviceProvider)
    {
        _daprPlacementHost = daprPlacementHost;
        _logger = logger;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await Task.Run(
            async () =>
            {
                var processInfo = _daprPlacementHost.GetProcessInfo();
                while (!processInfo.IsRunning && processInfo.Status != DaprProcessStatus.Disabled)
                {
                    _logger.LogInformation("Dapr Sidecar process is waiting for the Dapr Placement process to finish starting up...");
                    await Task.Delay(250);
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }

                    processInfo = _daprPlacementHost.GetProcessInfo();
                }
            }, cancellationToken)
            .ContinueWith(_ => base.StartAsync(cancellationToken));
    }
}
