using Volo.Abp.Domain.Entities.Auditing;

namespace MiniJob.Jobs;

/// <summary>
/// Worker 信息
/// </summary>
public class WorkerInfo : AuditedAggregateRoot<Guid>, IComparable<WorkerInfo>
{
    /// <summary>
    /// 地址 ip:port
    /// </summary>
    public virtual string Address { get; set; }

    /// <summary>
    /// 最后活动时间
    /// </summary>
    public virtual DateTime LastActiveTime { get; set; }

    /// <summary>
    /// 客户端名称
    /// </summary>
    public virtual string Client { get; set; }

    /// <summary>
    /// 标签
    /// </summary>
    public virtual string Tags { get; set; }

    /// <summary>
    /// CPU核数
    /// </summary>
    public virtual int CpuCores { get; set; }

    /// <summary>
    /// CPU总使用率
    /// </summary>
    public virtual double CpuUsed { get; set; }

    /// <summary>
    /// 内存使用量 GB
    /// </summary>
    public virtual double MemoryUsed { get; set; }

    /// <summary>
    /// 内存总量 GB
    /// </summary>
    public virtual double MemoryTotal { get; set; }

    /// <summary>
    /// 磁盘使用量 GB
    /// </summary>
    public virtual double DiskUsed { get; set; }

    /// <summary>
    /// 磁盘总量 GB
    /// </summary>
    public virtual double DiskTotal { get; set; }

    protected WorkerInfo() { }

    public WorkerInfo(Guid id, string address)
        : base(id)
    {
        Address = address;
    }

    /// <summary>
    /// 基于CPU和内存信息计算分数
    /// </summary>
    /// <returns></returns>
    public virtual int CalculateScore()
    {
        var memoryScore = (MemoryTotal - MemoryUsed) * 2;
        var cpuScore = CpuCores * (1 - CpuUsed);

        // 如果没有采集到CPU使用率则设置CPU分数为1
        if (cpuScore >= CpuCores)
            cpuScore = 1;

        return (int)(memoryScore + cpuScore);
    }

    /// <summary>
    /// 判断机器是否可用
    /// </summary>
    /// <param name="minCpuCores">最低CPU核心数量</param>
    /// <param name="minMemorySpace">最低内存空间</param>
    /// <param name="minDiskSpace">最低磁盘空间</param>
    /// <returns></returns>
    public virtual bool Available(double minCpuCores, double minMemorySpace, double minDiskSpace)
    {
        var availableMemory = MemoryTotal - MemoryUsed;
        var availableDisk = DiskUsed - DiskUsed;

        if (availableMemory < minMemorySpace || availableDisk < minDiskSpace)
            return false;

        if (CpuUsed <= 0 || minCpuCores <= 0)
            return true;

        return minCpuCores < CpuCores * (1 - CpuUsed);
    }

    /// <summary>
    /// 是否超时，超时时长为 <see cref="MiniJobConsts.WorkerTimeout"/>
    /// </summary>
    /// <returns></returns>
    public virtual bool Timeout()
    {
        return MiniJobConsts.WorkerTimeout > DateTime.Now - LastActiveTime;
    }

    public int CompareTo(WorkerInfo other)
    {
        // 按指标降序排序
        return other.CalculateScore() - this.CalculateScore();
    }
}