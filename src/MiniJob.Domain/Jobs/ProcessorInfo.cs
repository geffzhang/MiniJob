using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace MiniJob.Jobs
{
    /// <summary>
    /// 执行器信息
    /// 包括内置执行器和Worker报告的执行器信息
    /// </summary>
    public class ProcessorInfo : AuditedAggregateRoot<Guid>
    {
        /// <summary>
        /// 执行器全称
        /// 如：Mini.Job.Worker.Executors.HttpExecutor
        /// </summary>
        public virtual string FullName { get; set; }

        /// <summary>
        /// Worker 程序集名称
        /// </summary>
        public virtual string WorkerName { get; set; }

        /// <summary>
        /// 标签,多个用逗号分隔
        /// </summary>
        public virtual string Tags { get; set; }

        /// <summary>
        /// 是否内置的执行器(内置执行器可以选择在Server上执行)
        /// </summary>
        public virtual bool IsBuiltInExecutor { get; set; }

        /// <summary>
        /// 是否可用
        /// </summary>
        public virtual bool IsEnabled { get; set; }

        protected ProcessorInfo() { }

        public ProcessorInfo(Guid id, string fullName, string workerName)
            : base(id)
        {
            FullName = fullName;
            WorkerName = workerName;
        }

        public override string ToString()
        {
            return $"{base.ToString()}, FullName = {FullName}";
        }
    }
}