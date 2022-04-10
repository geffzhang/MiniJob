using MiniJob.Dapr.Security;

namespace MiniJob.Dapr.Options;

/// <summary>
/// Dapr 进程配置选项
/// </summary>
public abstract class DaprProcessOptions
{
    /// <summary>
    /// 获取或设置包含进程二进制文件的目录的完整路径
    /// </summary>
    /// <remarks>
    /// 如果指定了并且在该目录下找不到二进制文件，且 <see cref="CopyProcessFile"/> 为 <c>true</c>，则进程二进制文件将从 <see cref="RuntimeDirectory"/>/bin拷贝到该目录
    /// <para>如果没有指定，默认为 <see cref="RuntimeDirectory"/>/bin</para>
    /// </remarks>
    public string BinDirectory { get; set; }

    /// <summary>
    /// 获取或设置一个值，该值决定当Dapr进程二进制文件不同时是否从 <see cref="InitialDirectory"/> 复制到 <see cref="BinDirectory"/>
    /// </summary>
    /// <remarks>
    /// 这允许在应用程序运行时将新版本的二进制文件部署到 <see cref="InitialDirectory"/>，这样在重新启动应用程序时就会获得新版本
    /// <para>默认为 <c>false</c></para>
    /// </remarks>
    public bool? CopyProcessFile { get; set; }

    /// <summary>
    /// 获取或设置一个值，该值确定是否启用了Dapr进程。如果 <c>false</c>， Dapr进程二进制在启动时将不会被启动或管理
    /// </summary>
    /// <remarks>
    /// 这允许应用程序使用 MiniJob.Dapr 进行开发，同时利用标准的Dapr运行时机制进行部署
    /// <para>默认为 <c>true</c></para>
    /// </remarks>
    public bool? Enabled { get; set; }

    /// <summary>
    /// 获取或设置包含Dapr组件的初始目录的完整路径
    /// </summary>
    /// <remarks>
    /// 通常，这个目录是由 "dapr init" 命令创建的，包含 config.yaml 文件、bin、components和certs子目录
    /// <para>如果没有指定，默认为 %USERPROFILE%/.dapr (Linux 为 $HOME/.dapr)</para>
    /// </remarks>
    public string InitialDirectory { get; set; }

    /// <summary>
    /// 获取或设置用于mTLS加密的颁发者证书
    /// </summary>
    public SensitiveString IssuerCertificate { get; set; }

    /// <summary>
    /// 获取或设置用于mTLS证书的颁发者私钥
    /// </summary>
    public SensitiveString IssuerKey { get; set; }

    /// <summary>
    /// 获取或设置日志级别，可选项有debug、info、warning、error或fatal(默认为info)
    /// </summary>
    public string LogLevel { get; set; }

    /// <summary>
    /// 获取或设置用于管理和充实由Dapr二进制文件公开指标的配置选项
    /// </summary>
    public DaprMetricsOptions Metrics { get; set; }

    /// <summary>
    /// 获取或设置Dapr进程二进制文件的文件名的完整路径
    /// </summary>
    /// <remarks>
    /// 如果没有指定，默认为 <see cref="BinDirectory"/>/<see cref="ProcessName"/>.exe (Linux为 <see cref="BinDirectory"/>/<see cref="ProcessName"/>)
    /// </remarks>
    public string ProcessFile { get; set; }

    /// <summary>
    /// 获取或设置用于确定 <see cref="ProcessFile"/> 和标识重复/可附加实例的进程的名称
    /// <para>如果没有指定，将使用该进程的适当默认值</para>
    /// </summary>
    public string ProcessName { get; set; }

    /// <summary>
    /// 获取或设置在Dapr sidecar启动失败或意外退出时尝试重新启动的时间间隔
    /// <para>设置为任何负数禁用重试，默认为5000(5秒)</para>
    /// </summary>
    public int? RestartAfterMillseconds { get; set; }

    /// <summary>
    /// 获取或设置一个值，该值确定重启时是否保留自动分配的端口
    /// </summary>
    /// <remarks>
    /// 如果设为 false，端口分配将被更新为使用操作系统报告的下一个可用端口集
    /// <para>默认为 true，以确保在重启时保留现有的端口分配</para>
    /// </remarks>
    public bool? RetainPortsOnRestart { get; set; }

    /// <summary>
    /// 获取或设置Dapr组件的运行时目录的完整路径
    /// <para>如果没有指定，默认为 <see cref="InitialDirectory"/></para>
    /// </summary>
    public string RuntimeDirectory { get; set; }

    /// <summary>
    /// 获取或设置一个值，该值确定在强制杀死进程之前等待进程正常关闭的秒数
    /// <para>如果没有指定，进程将被立即强制终止</para>
    /// </summary>
    public int? WaitForShutdownSeconds { get; set; }

    /// <summary>
    /// Gets or sets the trust anchor certificate used for mTLS encryption.
    /// </summary>
    public SensitiveString TrustAnchorsCertificate { get; set; }

    /// <summary>
    /// 获取或设置一组要在进程启动时设置的环境变量
    /// <para>这些设置将覆盖从其他配置设置计算出的任何其他环境变量</para>
    /// </summary>
    public Dictionary<string, string> EnvironmentVariables { get; set; }

    /// <summary>
    /// 创建此实例的深度克隆
    /// </summary>
    /// <returns>这个实例的深度克隆</returns>
    public DaprProcessOptions Clone()
    {
        var clone = (DaprProcessOptions)MemberwiseClone();
        clone.Metrics = Metrics?.Clone();
        return clone;
    }

    /// <summary>
    /// 用 <paramref name="source"/> 中指定的值更新此实例中任何未定义的属性
    /// </summary>
    /// <param name="source">源选项实例</param>
    public void EnrichFrom(DaprProcessOptions source)
    {
        if (source == null)
        {
            return;
        }

        BinDirectory ??= source.BinDirectory;
        CopyProcessFile ??= source.CopyProcessFile;
        Enabled ??= source.Enabled;
        InitialDirectory ??= source.InitialDirectory;
        IssuerCertificate ??= source.IssuerCertificate;
        IssuerKey ??= source.IssuerKey;
        LogLevel ??= source.LogLevel;
        ProcessFile ??= source.ProcessFile;
        ProcessName ??= source.ProcessName;
        RestartAfterMillseconds ??= source.RestartAfterMillseconds;
        RetainPortsOnRestart ??= source.RetainPortsOnRestart;
        RuntimeDirectory ??= source.RuntimeDirectory;
        WaitForShutdownSeconds ??= source.WaitForShutdownSeconds;
        TrustAnchorsCertificate ??= source.TrustAnchorsCertificate;

        (Metrics ??= new DaprMetricsOptions()).EnrichFrom(source.Metrics);

        // Copy the environment variables - do it the old-fashioned way so we overwrite existing values
        // without getting key duplication errors.
        if (source.EnvironmentVariables?.Any() == true)
        {
            if (EnvironmentVariables == null)
            {
                EnvironmentVariables = new Dictionary<string, string>();
            }

            foreach (var entry in source.EnvironmentVariables)
            {
                EnvironmentVariables[entry.Key] = entry.Value;
            }
        }
    }

    /// <summary>
    /// 获取健康检查端点地址, 例如 http://127.0.0.1:3500/v1.0/health.
    /// </summary>
    /// <returns>健康检查端点地址</returns>
    public Uri GetHealthUri() => GetLocalUri(builder => AddHealthUri(builder));

    /// <summary>
    /// 获取 metrics 端点地址, 例如 http://127.0.0.1:9090.
    /// </summary>
    /// <returns>metrics 端点地址</returns>
    public Uri GetMetricsUri() => GetLocalUri(builder => AddMetricsUri(builder));

    protected virtual bool AddHealthUri(UriBuilder builder) => false;

    protected virtual bool AddMetricsUri(UriBuilder builder) => false;

    protected static Uri GetLocalUri(Func<UriBuilder, bool> configure)
    {
        var builder = new UriBuilder("http", DaprConstants.LocalhostAddress);
        if (configure(builder))
        {
            return builder.Uri;
        }
        else
        {
            return null;
        }
    }
}
