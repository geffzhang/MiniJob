namespace MiniJob.Dapr;

/// <summary>
/// Dapr Sentry 配置选项
/// </summary>
public class DaprSentryOptions : Options.DaprProcessOptions
{
    public DaprSentryOptions()
    {
        HealthPort = 8080;
    }

    /// <summary>
    /// 获取或设置Dapr配置文件的路径。如果没有指定文件名，则使用默认值 "config.yaml"
    /// </summary>
    public string ConfigFile { get; set; }

    /// <summary>
    /// 获取或设置该流程的自定义参数。这些参数将“按原样”附加在通过这些配置选项指定的所有其他参数之后
    /// </summary>
    public string CustomArguments { get; set; }

    /// <summary>
    /// 获取或设置一个值，该值决定是否在Placement服务中启用Prometheus指标(默认为true)
    /// </summary>
    public bool? EnableMetrics { get; set; }

    /// <summary>
    /// 获取或设置健康检查的HTTP端口(默认8080)
    /// </summary>
    public int? HealthPort { get; internal set; } // Internal for now for testing, as cannot yet be set on sentry.exe

    /// <summary>
    /// 获取或设置保存颁发者数据的凭据目录的路径
    /// <para>如果没有指定，它将默认放置在运行时文件夹下名为"certs"的目录下</para>
    /// </summary>
    public string CertsDirectory { get; set; }

    /// <summary>
    /// 获取或设置 metrics 端口(默认为9092)
    /// </summary>
    public int? MetricsPort { get; set; }

    /// <summary>
    /// 获取或设置CA信任域(默认为“localhost”)
    /// </summary>
    public string TrustDomain { get; set; }

    /// <summary>
    /// 创建此实例的深度克隆
    /// </summary>
    /// <returns>这个实例的深度克隆</returns>
    public new DaprSentryOptions Clone() => (DaprSentryOptions)base.Clone();

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
