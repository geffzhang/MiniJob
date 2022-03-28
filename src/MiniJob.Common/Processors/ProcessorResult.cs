namespace MiniJob.Processors;

/// <summary>
/// 处理结果
/// </summary>
public class ProcessorResult
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public virtual bool Success { get; set; }

    /// <summary>
    /// 消息内容
    /// </summary>
    public virtual string Message { get; set; }

    public static ProcessorResult OK = new(true);

    public static ProcessorResult Error = new(false);

    public static ProcessorResult OkMessage(string message) => new(true, message);

    public static ProcessorResult ErrorMessage(string message) => new(false, message);

    public ProcessorResult(bool success)
    {
        Success = success;
    }

    public ProcessorResult(bool success, string message)
    {
        Success = success;
        Message = message;
    }

    public override string ToString()
    {
        return $"{(Success ? "Success" : "Failed")}{(string.IsNullOrWhiteSpace(Message) ? "" : $":{Message}")}";
    }
}