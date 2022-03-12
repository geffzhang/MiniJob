using System;

namespace MiniJob.Model
{
    /// <summary>
    /// 系统指标
    /// </summary>
    [Serializable]
    public class SystemMetrics : IComparable<SystemMetrics>
    {
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

        /// <summary>
        /// 分数
        /// </summary>
        public virtual double Score { get; set; }

        /// <summary>
        /// 基于内存和CPU信息计算分数
        /// </summary>
        /// <returns></returns>
        public double CalculateScore()
        {
            if (Score > 0)
            {
                return Score;
            }

            var memoryScore = MemoryTotal - MemoryUsed;
            var cpuScore = (1 - CpuUsed) * CpuCores;
            if (cpuScore >= CpuCores)
            {
                cpuScore = 1;
            }

            return Math.Round(memoryScore + cpuScore, 2);
        }

        /// <summary>
        /// 判断机器是否可用
        /// </summary>
        /// <param name="minCPUCores"></param>
        /// <param name="minMemorySpace"></param>
        /// <param name="minDiskSpace"></param>
        /// <returns></returns>
        public bool Available(double minCPUCores, double minMemorySpace, double minDiskSpace)
        {
            double availableMemory = MemoryTotal - MemoryUsed;
            double availableDisk = DiskTotal - DiskUsed;

            if (availableMemory < minMemorySpace || availableDisk < minDiskSpace)
            {
                return false;
            }

            if (CpuUsed <= 0 || minCPUCores <= 0)
            {
                return true;
            }
            return minCPUCores < CpuCores * (1 - CpuUsed);
        }

        public int CompareTo(SystemMetrics other)
        {
            // 按分数倒序排序
            return (int)(other.CalculateScore() - this.CalculateScore());
        }
    }
}
