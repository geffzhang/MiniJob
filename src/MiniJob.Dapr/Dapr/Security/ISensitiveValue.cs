namespace MiniJob.Dapr.Security;

/// <summary>
/// 表示一个包含敏感值的对象
/// </summary>
public interface ISensitiveValue
{
    /// <summary>
    /// 敏感值
    /// </summary>
    object Value { get; }
}
