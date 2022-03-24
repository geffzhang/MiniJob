using MiniJob.Entities;
using System.Diagnostics.CodeAnalysis;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace MiniJob.Jobs;

/// <summary>
/// 任务运行实例
/// </summary>
public class JobInstance : AuditedAggregateRoot<Guid>, IMultiTenant
{
    /// <summary>
    /// 租户ID
    /// </summary>
    public virtual Guid? TenantId { get; protected set; }

    /// <summary>
    /// 应用ID
    /// </summary>
    public virtual Guid AppId { get; protected set; }

    /// <summary>
    /// 任务
    /// </summary>
    public virtual JobInfo JobInfo { get; set; }

    /// <summary>
    /// 任务ID
    /// </summary>
    public virtual Guid JobInfoId { get; protected set; }

    /// <summary>
    /// 任务实例参数
    /// </summary>
    public virtual string InstanceArgs { get; set; }

    /// <summary>
    /// 任务状态
    /// </summary>
    public virtual InstanceStatus InstanceStatus { get; set; }

    /// <summary>
    /// 执行结果（允许存储稍大的结果）
    /// </summary>
    public virtual string Result { get; set; }

    /// <summary>
    /// 预计触发时间
    /// </summary>
    public virtual DateTime ExpectedTriggerTime { get; set; }

    /// <summary>
    /// 实际触发时间
    /// </summary>
    public virtual DateTime? ActualTriggerTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public virtual DateTime? FinishedTime { get; set; }

    /// <summary>
    /// 重试次数
    /// </summary>
    public virtual int TryCount { get; set; }

    protected JobInstance()
    {
    }

    public JobInstance(
        Guid id,
        Guid appid,
        Guid jobId,
        DateTime nextTriggerTime,
        string instanceArgs = null,
        [MaybeNull] Guid? tenantId = null)
        : base(id)
    {
        AppId = appid;
        JobInfoId = jobId;
        TenantId = tenantId;
        ExpectedTriggerTime = nextTriggerTime;
        InstanceArgs = instanceArgs;
        InstanceStatus = InstanceStatus.WaitingDispatch;
    }

    public override string ToString()
    {
        return $"{base.ToString()}, JobId = {JobInfoId}, Status = {InstanceStatus}";
    }
}