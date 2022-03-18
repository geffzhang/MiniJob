namespace MiniJob.Processors;

/// <summary>
/// Http 参数
/// </summary>
public class HttpArgs
{
    /// <summary>
    /// POST / GET / PUT / DELETE
    /// </summary>
    public string Method { get; set; }

    /// <summary>
    /// 请求的Url
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// application/json
    /// </summary>
    public string MediaType { get; set; }

    /// <summary>
    /// 请求体
    /// </summary>
    public string Body { get; set; }

    /// <summary>
    /// 请求头
    /// </summary>
    public Dictionary<string, string> Headers { get; set; }

    /// <summary>
    /// 超时时长（秒），默认为60秒
    /// </summary>
    public int Timeout { get; set; }

    /// <summary>
    /// 返回校验key
    /// </summary>
    public string CheckKey { get; set; }

    /// <summary>
    /// 返回校验value
    /// </summary>
    public string CheckValue { get; set; }

    public HttpArgs()
    {
        Method = "GET";
        MediaType = "application/json";
        Timeout = 60;
    }
}
