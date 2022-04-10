namespace MiniJob.Dapr;

/// <summary>
/// Dapr Placement 配置选项
/// </summary>
public class DaprPlacementOptions : Options.DaprProcessOptions
{
    /// <summary>
    /// 获取或设置保存颁发者数据的凭据目录的路径
    /// <para>如果没有指定，它将默认放置在运行时文件夹下名为"certs"的目录下</para>
    /// </summary>
    public string CertsDirectory { get; set; }

    /// <summary>
    /// 获取或设置该进程的自定义参数。这些参数将“按原样”附加在通过这些配置选项指定的所有其他参数之后
    /// </summary>
    public string CustomArguments { get; set; }

    /// <summary>
    /// 获取或设置一个值，该值决定是否在Placement服务中启用Prometheus指标(默认为true)
    /// </summary>
    public bool? EnableMetrics { get; set; }

    /// <summary>
    /// 获取或设置健康检查的HTTP端口(默认8081)
    /// </summary>
    public int? HealthPort { get; set; }

    /// <summary>
    /// 获取或设置 placement 服务 ID (默认为 "dapr-placement-0").
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 获取或设置 raft 集群对等点 (默认为 "dapr-placement-0=127.0.0.1:8201").
    /// </summary>
    public string InitialCluster { get; set; }

    /// <summary>
    /// 获取或设置一个值，该值确定是否启用内存日志和快照存储，除非设置了raft-logstore-path(默认为true)
    /// </summary>
    public bool? InmemStoreEnabled { get; set; }

    /// <summary>
    /// 获取或设置 metrics 端口(默认为9091)
    /// </summary>
    public int? MetricsPort { get; set; }

    /// <summary>
    /// 获取或设置一个值，该值决定是否应为 placement gRPC 服务启用TLS
    /// </summary>
    public bool? Mtls { get; set; }

    /// <summary>
    /// 获取或设置 placement 服务的gRPC端口(在Windows上默认为6050，在其他平台上默认为50005)
    /// </summary>
    public int? Port { get; set; }

    /// <summary>
    /// 获取或设置 raft 日志存储库的路径
    /// </summary>
    public string RaftLogstorePath { get; set; }

    /// <summary>
    /// 获取或设置vnode上 actor 分布的复制因子(默认100)
    /// </summary>
    public int? ReplicationFactor { get; set; }

    /// <summary>
    /// 创建此实例的深度克隆
    /// </summary>
    /// <returns>这个实例的深度克隆</returns>
    public new DaprPlacementOptions Clone() => (DaprPlacementOptions)base.Clone();

    protected override bool AddHealthUri(UriBuilder builder)
    {
        if (!HealthPort.HasValue)
        {
            return false;
        }

        builder.Port = HealthPort.Value;
        builder.Path = "healthz";
        return true;
    }

    protected override bool AddMetricsUri(UriBuilder builder)
    {
        if (!MetricsPort.HasValue || EnableMetrics == false)
        {
            return false;
        }

        builder.Port = MetricsPort.Value;
        return true;
    }
}
