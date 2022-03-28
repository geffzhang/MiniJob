namespace MiniJob.Processors;

public class TaskResult
{
    public virtual Guid TaskId { get; set; }

    public virtual bool Success { get; set; }

    public virtual string Message { get; set; }
}
