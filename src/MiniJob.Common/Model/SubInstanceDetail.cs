using MiniJob.Entities;

namespace MiniJob.Model;

[Serializable]
public class SubInstanceDetail
{
    public virtual Guid SubInstanceId { get; set; }

    public virtual DateTime StartTime { get; set; }

    public virtual DateTime FinishedTime { get; set; }

    public virtual string Result { get; set; }

    public virtual InstanceStatus Status { get; set; }
}