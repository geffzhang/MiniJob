namespace MiniJob.Dapr;

/// <summary>
/// Dapr Metrics 配置选项
/// </summary>
public class DaprMetricsOptions
{
    /// <summary>
    /// 获取或设置一个值，该值确定是否为此组件启用指标收集器(默认为true)
    /// </summary>
    public bool? EnableCollector { get; set; }

    /// <summary>
    /// 获取或设置要添加到每个度量行的标签
    /// </summary>
    public IDictionary<string, string> Labels { get; set; }

    /// <summary>
    /// 创建此实例的深度克隆
    /// </summary>
    /// <returns>这个实例的深度克隆</returns>
    public DaprMetricsOptions Clone() => (DaprMetricsOptions)MemberwiseClone();

    /// <summary>
    /// 用 <paramref name="source"/> 中指定的值更新此实例中任何未定义的属性
    /// </summary>
    /// <param name="source">源选项实例</param>
    public void EnrichFrom(DaprMetricsOptions source)
    {
        if (source == null)
        {
            return;
        }

        EnableCollector ??= source.EnableCollector;

        if (source.Labels != null)
        {
            foreach (var label in source.Labels)
            {
                SetLabel(label.Key, label.Value);
            }
        }
    }

    /// <summary>
    /// 设置标签
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="overwrite"></param>
    public void SetLabel(string name, string value, bool overwrite = false)
    {
        if (string.IsNullOrEmpty(name))
        {
            return;
        }

        if (Labels == null)
        {
            Labels = new Dictionary<string, string>();
        }

        // 如果标签不存在或指定了覆盖标志，则设置该标签
        if (!Labels.ContainsKey(name) || overwrite)
        {
            Labels[name] = value;
        }
    }
}
