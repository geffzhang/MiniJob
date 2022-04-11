using Microsoft.Extensions.Diagnostics.HealthChecks;
using MiniJob.Dapr.Processes;

namespace MiniJob.Dapr.AspNetCore;

/// <summary>
/// Dapr 进程健康检查
/// </summary>
public abstract class DaprProcessHealthCheck : IHealthCheck
{
    private readonly IDaprProcessHost _daprProcessHost;

    protected DaprProcessHealthCheck(IDaprProcessHost daprProcessHost)
    {
        _daprProcessHost = daprProcessHost;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            // 确保进程已启用
            var processInfo = _daprProcessHost.GetProcessInfo();
            if (processInfo.Status != DaprProcessStatus.Disabled)
            {
                // 检查进程是否正在运行或附加
                if (!processInfo.IsRunning && !processInfo.IsAttached)
                {
                    return new HealthCheckResult(context.Registration.FailureStatus, description: processInfo.Description);
                }

                // 检查健康检查端点
                var result = await _daprProcessHost.GetHealthAsync(cancellationToken);
                if (!result.IsHealthy)
                {
                    // 不健康
                    return new HealthCheckResult(context.Registration.FailureStatus, "Dapr process health check endpoint reports Unhealthy (" + result.StatusCode + ")");
                }
            }

            return HealthCheckResult.Healthy(description: processInfo.Description);
        }
        catch (Exception ex)
        {
            return new HealthCheckResult(context?.Registration?.FailureStatus ?? HealthStatus.Unhealthy, exception: ex);
        }
    }
}
