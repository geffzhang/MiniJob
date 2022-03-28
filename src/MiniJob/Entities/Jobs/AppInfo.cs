using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace MiniJob.Entities.Jobs;

/// <summary>
/// 应用信息，用于分组
/// </summary>
public class AppInfo : AuditedAggregateRoot<Guid>, IMultiTenant
{
    /// <summary>
    /// 租户ID
    /// </summary>
    public virtual Guid? TenantId { get; set; }

    /// <summary>
    /// 应用名称
    /// </summary>
    public virtual string AppName { get; set; }

    /// <summary>
    /// 应用描述
    /// </summary>
    public virtual string Description { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public virtual bool IsEnabled { get; set; }

    /// <summary>
    /// 应用的任务信息
    /// </summary>
    public virtual ICollection<JobInfo> JobInfos { get; set; }

    /// <summary>
    /// 应用的执行器信息
    /// </summary>
    public virtual ICollection<ProcessorInfo> ProcessorInfos { get; set; }

    protected AppInfo() { }

    public AppInfo(
        [NotNull] Guid id,
        [NotNull] string appName,
        [MaybeNull] string description,
        [MaybeNull] Guid? tenantId = null)
        : base(id)
    {
        TenantId = tenantId;
        AppName = appName;
        Description = description;
        IsEnabled = true;

        JobInfos = new Collection<JobInfo>();
        ProcessorInfos = new Collection<ProcessorInfo>();
    }

    public virtual void Disable()
    {
        IsEnabled = false;
    }

    public virtual void Enable()
    {
        IsEnabled = true;
    }

    public override string ToString()
    {
        return $"{base.ToString()}, AppName = {AppName}";
    }
}