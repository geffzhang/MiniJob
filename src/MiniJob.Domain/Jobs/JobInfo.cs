using MiniJob.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace MiniJob.Jobs
{
    /// <summary>
    /// 任务信息
    /// </summary>
    public class JobInfo : AuditedAggregateRoot<Guid>, IMultiTenant
    {
        #region 任务基本信息
        /// <summary>
        /// 租户ID
        /// </summary>
        public virtual Guid? TenantId { get; protected set; }

        /// <summary>
        /// 任务名称
        /// </summary>
        public virtual string JobName { get; protected internal set; }

        /// <summary>
        /// 任务描述
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// 任务自带的参数
        /// </summary>
        public virtual string JobArgs { get; protected internal set; }

        /// <summary>
        /// 任务优先级
        /// </summary>
        public virtual JobPriority JobPriority { get; set; }

        /// <summary>
        /// 标签,多个用逗号分隔
        /// </summary>
        public virtual string Tags { get; set; }
        #endregion

        #region 定时参数
        /// <summary>
        /// 时间表达式类型
        /// </summary>
        public virtual TimeExpressionType TimeExpression { get; set; }

        /// <summary>
        /// 时间表达式值
        /// </summary>
        public virtual string TimeExpressionValue { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public virtual DateTime? BeginTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public virtual DateTime? EndTime { get; set; }
        #endregion

        #region 执行方式
        /// <summary>
        /// 执行类型(单机/广播/分片等)
        /// </summary>
        public virtual ExecuteType ExecuteType { get; set; }

        /// <summary>
        /// 执行器类型(Shell/CSharp/Python/SQL等)
        /// </summary>
        public virtual ProcessorType ProcessorType { get; set; }

        /// <summary>
        /// 执行器信息（执行器全称FullName）
        /// </summary>
        public virtual string ExecutorInfo { get; set; }
        #endregion

        #region 运行时配置
        /// <summary>
        /// 最大同时运行任务数,默认1，0 代表不限制实例数量
        /// </summary>
        public virtual int MaxInstanceCount { get; set; }

        /// <summary>
        /// 并发度,同时执行某个任务的最大线程数量（MapReduce 任务生效）
        /// </summary>
        public virtual int Concurrency { get; set; }

        /// <summary>
        /// 任务整体超时时间
        /// </summary>
        public virtual TimeSpan Timeout { get; set; }
        #endregion

        #region 重试配置
        /// <summary>
        /// 最大重试次数
        /// </summary>
        public virtual int MaxTryCount { get; set; }

        /// <summary>
        /// 下次调度时间
        /// </summary>
        public virtual DateTime? NextTriggerTime { get; set; }

        /// <summary>
        /// 最后一次调度时间
        /// </summary>
        public virtual DateTime? LastTriggerTime { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public virtual bool IsEnabled { get; set; }

        /// <summary>
        /// 过期策略(忽略、立即触发补偿一次)，默认为忽略
        /// </summary>
        public virtual MisfireStrategy MisfireStrategy { get; set; }
        #endregion

        #region 繁忙机器配置
        /// <summary>
        /// 最低CPU核心数量,0代表不限制
        /// </summary>
        public virtual double MinCpuCores { get; set; }

        /// <summary>
        /// 最低内存空间,单位GB,0代表不限制
        /// </summary>
        public virtual double MinMemorySpace { get; set; }

        /// <summary>
        /// 最低磁盘空间,单位GB,0代表不限制
        /// </summary>
        public virtual double MinDiskSpace { get; set; }
        #endregion

        #region 集群配置
        /// <summary>
        /// 最大执行机器数量
        /// </summary>
        public virtual int MaxWorkerCount { get; set; }

        #endregion

        /// <summary>
        /// 任务运行信息
        /// </summary>
        public virtual ICollection<JobInstance> JobInstances { get; set; }

        /// <summary>
        /// 告警用户
        /// </summary>
        public virtual ICollection<JobInfoIdentityUser> AlarmUsers { get; private set; }

        protected JobInfo()
        {
        }

        public JobInfo(
            [NotNull] Guid id,
            [NotNull] string jobName,
            [MaybeNull] string jobArgs,
            [MaybeNull] Guid? tenantId = null)
            : base(id)
        {
            TenantId = tenantId;
            JobName = jobName;
            JobArgs = jobArgs;
            IsEnabled = true;

            JobPriority = JobPriority.Normal;
            MisfireStrategy = MisfireStrategy.Ignore;
            MaxInstanceCount = 1;
            MaxTryCount = 3;

            JobInstances = new Collection<JobInstance>();
        }

        public virtual void Disable()
        {
            IsEnabled = false;
        }

        public virtual void Enable()
        {
            IsEnabled = true;
        }

        public void AddAlarmUser(Guid userId)
        {
            Check.NotNull(userId, nameof(userId));

            if (IsInAlarmUsers(userId))
            {
                return;
            }

            AlarmUsers.Add(new JobInfoIdentityUser(Id, userId));
        }

        public void RemoveAlarmUser(Guid userId)
        {
            Check.NotNull(userId, nameof(userId));

            if (!IsInAlarmUsers(userId))
            {
                return;
            }

            AlarmUsers.RemoveAll(x => x.UserId == userId);
        }


        public void RemoveAllAlarmUsers()
        {
            AlarmUsers.RemoveAll(x => x.JobInfoId == Id);
        }

        private bool IsInAlarmUsers(Guid userId)
        {
            return AlarmUsers.Any(x => x.UserId == userId);
        }

        public override string ToString()
        {
            return $"{base.ToString()}, JobName = {JobName}";
        }
    }
}