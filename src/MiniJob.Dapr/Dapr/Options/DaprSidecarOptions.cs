using MiniJob.Dapr.Security;

namespace MiniJob.Dapr;

/// <summary>
/// Dapr Sidecar 配置选项
/// </summary>
public class DaprSidecarOptions : Options.DaprProcessOptions
{
    public DaprSidecarOptions()
    {
        // 将默认关机等待时间设置为10秒，以便sidecar有机会响应shutdown命令
        WaitForShutdownSeconds = 10;
    }

    /// <summary>
    /// 获取或设置允许的HTTP源(默认为"*")
    /// </summary>
    public string AllowedOrigins { get; set; }

    /// <summary>
    /// 获取或设置API令牌，该令牌将作为从sidecar发送到应用程序的每个请求的头部提供
    /// <para>当指定时，环境变量APP_API_TOKEN将被设置为这个值</para>
    /// See https://docs.dapr.io/operations/security/app-api-token/.
    /// </summary>
    public SensitiveString AppApiToken { get; set; }

    /// <summary>
    /// 获取或设置Dapr的唯一ID。用于服务发现
    /// </summary>
    public string AppId { get; set; }

    /// <summary>
    /// 获取或设置一个值，该值在将请求转发给用户代码时控制并发级别(默认为-1)
    /// </summary>
    public int? AppMaxConcurrency { get; set; }

    /// <summary>
    /// 获取或设置应用程序侦听的端口
    /// </summary>
    public int? AppPort { get; set; }

    /// <summary>
    /// 获取或设置应用程序的回调协议:grpc或http(默认为"http")
    /// </summary>
    public string AppProtocol { get; set; }

    /// <summary>
    /// 获取或设置一个值，该值决定sidecar是否应该将应用程序的URI方案设置为https并尝试SSL连接。默认为 false
    /// </summary>
    public bool? AppSsl { get; set; }

    /// <summary>
    /// 获取或设置包含组件配置的Dapr Components目录的路径
    /// <para>如果没有指定，它将默认设置为当前dapr文件夹下的一个名为 components 的目录</para>
    /// </summary>
    public string ComponentsDirectory { get; set; }

    /// <summary>
    /// 获取或设置Dapr配置文件的路径。如果没有指定文件名，则使用默认值 "config.yaml"
    /// </summary>
    public string ConfigFile { get; set; }

    /// <summary>
    /// 获取或设置Dapr控制平面的地址
    /// </summary>
    public string ControlPlaneAddress { get; set; }

    /// <summary>
    /// 获取或设置该流程的自定义参数。这些参数将“按原样”附加在通过这些配置选项指定的所有其他参数之后
    /// </summary>
    public string CustomArguments { get; set; }

    /// <summary>
    /// 获取或设置API令牌，该令牌可望在每个公共API请求的头中看到，以验证调用者
    /// <para>当指定时，启动Dapr进程时，环境变量DAPR_API_TOKEN设置为这个值</para>
    /// See https://docs.dapr.io/operations/security/api-token/.
    /// </summary>
    public SensitiveString DaprApiToken { get; set; }

    /// <summary>
    /// 获取或设置用于侦听Dapr API的gRPC端口(默认为50001)
    /// </summary>
    public int? DaprGrpcPort { get; set; }

    /// <summary>
    /// 获取或设置请求体的最大大小(以MB为单位)，以处理大文件的上传(默认为4 MB)
    /// </summary>
    public int? DaprHttpMaxRequestSize { get; set; }

    /// <summary>
    /// 获取或设置用于侦听Dapr API的HTTP端口(默认为3500)
    /// </summary>
    public int? DaprHttpPort { get; set; }

    /// <summary>
    /// 获取或设置用于侦听Dapr内部API的gRPC端口
    /// </summary>
    public int? DaprInternalGrpcPort { get; set; }

    /// <summary>
    /// 获取或设置 kubeconfig 文件的绝对路径 (默认为 %USERPROFILE%/.kube/config).
    /// </summary>
    public string KubeConfig { get; set; }

    /// <summary>
    /// 获取或设置 metrics 端口(默认为9090)
    /// </summary>
    public int? MetricsPort { get; set; }

    /// <summary>
    /// 获取或设置Dapr sidecar的运行时模式(默认为 standalone)
    /// </summary>
    public string Mode { get; set; }

    /// <summary>
    /// 获取或设置一个值，该值确定是否为sidecar到sidecar通信通道启用自动mTLS。默认为 false
    /// </summary>
    public bool? Mtls { get; set; }

    /// <summary>
    /// 获取或设置此sidecar实例的名称空间
    /// 启动Dapr进程时，将环境变量NAMESPACE设置为这个值
    /// 如果未指定，则默认为 default
    /// </summary>
    /// <remarks>
    /// Dapr uses namespaces to determine which services can call other services. For example, components in the
    /// "development" namespace cannot call components in the "production" namespace.
    /// The NAMESPACE environment variable MUST be set when <see cref="Mtls"/> is enabled as it is encoded
    /// into the SPIFFE identifier.
    /// </remarks>
    public string Namespace { get; set; }

    /// <summary>
    /// 获取或设置 actor placement 服务主机地址。这通常是一个以逗号分隔的 host:port 端点列表
    /// </summary>
    public string PlacementHostAddress { get; set; }

    /// <summary>
    /// 获取或设置 profile 端口(默认为7777)
    /// </summary>
    public int? ProfilePort { get; set; }

    /// <summary>
    /// 获取或设置一个值，该值决定在Dapr Sidecar中是否启用分析功能。默认为 false
    /// </summary>
    public bool? Profiling { get; set; }

    /// <summary>
    /// 获取或设置哨兵CA服务的地址
    /// </summary>
    public string SentryAddress { get; set; }

    /// <summary>
    /// 获取或设置一个值，该值决定在 <see cref="AppApiToken"/> 未指定时是否生成默认API令牌。默认为 false
    /// </summary>
    public bool? UseDefaultAppApiToken { get; set; }

    /// <summary>
    /// 获取或设置一个值，该值决定在 <see cref="DaprApiToken"/> 未指定时是否生成默认API令牌。默认为 false
    /// </summary>
    public bool? UseDefaultDaprApiToken { get; set; }

    /// <summary>
    /// 获取或设置一个值，该值确定在未指定 <see cref="PlacementHostAddress"/> 时是否使用默认 placement 主机地址。默认为 true
    /// </summary>
    public bool? UseDefaultPlacementHostAddress { get; set; }

    /// <summary>
    /// 获取元数据端点的地址, 例如 http://127.0.0.1:3500/v1.0/metadata.
    /// </summary>
    /// <returns>The metadata endpoint address.</returns>
    public Uri GetMetadataUri() => GetLocalUri(builder => AddMetadataUri(builder));

    /// <summary>
    /// 获取关机端点的地址, 例如 http://127.0.0.1:3500/v1.0/shutdown.
    /// </summary>
    /// <returns>The metadata endpoint address.</returns>
    public Uri GetShutdownUri() => GetLocalUri(builder => AddShutdownUri(builder));

    /// <summary>
    /// 创建此实例的深度克隆
    /// </summary>
    /// <returns>这个实例的深度克隆</returns>
    public new DaprSidecarOptions Clone() => (DaprSidecarOptions)base.Clone();

    protected override bool AddHealthUri(UriBuilder builder)
    {
        if (!DaprHttpPort.HasValue)
        {
            return false;
        }

        builder.Port = DaprHttpPort.Value;
        builder.Path = "v1.0/healthz";
        return true;
    }

    protected override bool AddMetricsUri(UriBuilder builder)
    {
        if (!MetricsPort.HasValue)
        {
            return false;
        }

        builder.Port = MetricsPort.Value;
        return true;
    }

    private bool AddMetadataUri(UriBuilder builder)
    {
        if (!DaprHttpPort.HasValue)
        {
            return false;
        }

        builder.Port = DaprHttpPort.Value;
        builder.Path = "v1.0/metadata";
        return true;
    }

    private bool AddShutdownUri(UriBuilder builder)
    {
        if (!DaprHttpPort.HasValue)
        {
            return false;
        }

        builder.Port = DaprHttpPort.Value;
        builder.Path = "v1.0/shutdown";
        return true;
    }
}
