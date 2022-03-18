namespace MiniJob.Model;

[Serializable]
public class TaskDetail
{
    public virtual long TotalTaskNum { get; set; }

    public virtual long SucceedTaskNum { get; set; }

    public virtual long FailedTaskNum { get; set; }
}