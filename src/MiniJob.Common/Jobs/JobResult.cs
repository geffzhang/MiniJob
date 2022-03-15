namespace MiniJob.Jobs;

public class JobResult
{
    public virtual bool Success { get; set; }

    public virtual string Message { get; set; }

    public static JobResult OK = new(true);

    public static JobResult Error = new(false);

    public static JobResult OkMessage(string message) => new(true, message);

    public static JobResult ErrorMessage(string message) => new(false, message);

    public JobResult(bool success)
    {
        Success = success;
    }

    public JobResult(bool success, string message)
    {
        Success = success;
        Message = message;
    }

    public override string ToString()
    {
        return $"{(Success ? "Success" : "Failed")}{(string.IsNullOrWhiteSpace(Message) ? "" : $":{Message}")}";
    }
}