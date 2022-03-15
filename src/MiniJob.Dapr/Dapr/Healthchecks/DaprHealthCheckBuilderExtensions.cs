using MiniJob.Dapr.Healthchecks;

namespace Microsoft.Extensions.DependencyInjection;

public static class DaprHealthCheckBuilderExtensions
{
    /// <summary>
    /// 添加 Dapr sidecar 健康检查
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IHealthChecksBuilder AddDapr(this IHealthChecksBuilder builder) =>
        builder.AddCheck<DaprHealthCheck>("dapr");
}