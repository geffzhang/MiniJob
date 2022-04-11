using System.Net;

namespace MiniJob.Dapr.Processes;

/// <summary>
/// 对Dapr进程执行的健康检查的结果
/// </summary>
public class DaprHealthResult
{
    /// <summary>
    /// 获取具有未知状态的运行状况结果
    /// </summary>
    public static DaprHealthResult Unknown => new DaprHealthResult(HttpStatusCode.ServiceUnavailable, "Unknown or unreachable");

    public DaprHealthResult(HttpStatusCode statusCode, string statusDescription = null)
    {
        StatusCode = statusCode;
        StatusDescription = statusDescription;
    }

    /// <summary>
    /// 获取一个值，该值指示Dapr进程是否可达且正常
    /// </summary>
    public bool IsHealthy => (int)StatusCode >= 200 && (int)StatusCode < 400;

    public HttpStatusCode StatusCode { get; }

    public string StatusDescription { get; }

    public override string ToString() =>
        IsHealthy ? "Healthy" :
        string.IsNullOrEmpty(StatusDescription) ? string.Concat("Unhealthy (", StatusCode, ")") :
        string.Concat("Unhealthy (", StatusCode, ", ", StatusDescription, ")");
}
