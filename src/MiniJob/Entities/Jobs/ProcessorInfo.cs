﻿using Volo.Abp.Domain.Entities.Auditing;

namespace MiniJob.Entities.Jobs;

/// <summary>
/// 执行器信息
/// 包括内置执行器和Worker报告的执行器信息
/// </summary>
public class ProcessorInfo : AuditedAggregateRoot<Guid>
{
    public virtual Guid AppId { get; set; }

    /// <summary>
    /// 简短名称，如：HttpExecutor
    /// </summary>
    public virtual string Name { get; set; }

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