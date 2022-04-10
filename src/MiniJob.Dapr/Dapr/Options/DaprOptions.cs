namespace MiniJob.Dapr;

/// <summary>
/// Dapr 配置选项
/// </summary>
public class DaprOptions : Options.DaprProcessOptions
{
    public const string SectionName = "MiniJobDapr";
    public const string EnvironmentVariablePrefix = "MINIJOBDAPR_";

    public DaprOptions()
    {
        // 为所有组件设置默认值
        LogLevel = DaprConstants.DaprLogger.DebugLevel;
    }

    public DaprSidecarOptions Sidecar { get; set; }

    public DaprPlacementOptions Placement { get; set; }

    public DaprSentryOptions Sentry { get; set; }

    /// <summary>
    /// 创建此实例的深度克隆
    /// </summary>
    /// <returns>这个实例的深度克隆</returns>
    public new DaprOptions Clone()
    {
        var clone = (DaprOptions)base.Clone();
        clone.Sidecar = Sidecar?.Clone();
        clone.Placement = Placement?.Clone();
        clone.Sentry = Sentry?.Clone();
        return clone;
    }
}
