namespace MiniJob.Dapr.AspNetCore;

/// <summary>
/// 用于为 Dapr 注册附加服务的构建器
/// </summary>
public interface IDaprBuilder
{
    /// <summary>
    /// Adds the Dapr Placement service.
    /// </summary>
    /// <returns>This instance to allow calls to be chained.</returns>
    public IDaprBuilder AddPlacement();

    /// <summary>
    /// Adds the Dapr Sentry service.
    /// </summary>
    /// <returns>This instance to allow calls to be chained.</returns>
    public IDaprBuilder AddSentry();
}
