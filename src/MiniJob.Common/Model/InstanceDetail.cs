using MiniJob.Enums;

namespace MiniJob.Model;

/// <summary>
/// 任务实例明细
/// </summary>
[Serializable]
public class InstanceDetail
{
    public virtual DateTime ExpectedTriggerTime { get; set; }

    public virtual DateTime ActualTriggerTime { get; set; }

    public virtual DateTime FinishedTime { get; set; }

    public virtual InstanceStatus Status { get; set; }

    public virtual string Result { get; set; }

    public virtual string TaskTrackerAddress { get; set; }

    public virtual string JobParams { get; set; }

    public virtual string InstanceParams { get; set; }

    public virtual TaskDetail TaskDetail { get; set; }

    public virtual long RunningTimes { get; set; }

    public virtual string Extra { get; set; }

    public virtual List<SubInstanceDetail> SubInstanceDetails { get; set; }
}