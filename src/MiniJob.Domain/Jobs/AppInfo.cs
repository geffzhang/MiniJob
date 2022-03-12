using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace MiniJob.Jobs
{
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
    }
}
