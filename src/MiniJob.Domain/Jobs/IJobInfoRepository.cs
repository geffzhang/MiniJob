using Volo.Abp.Domain.Repositories;

namespace MiniJob.Jobs;

/// <summary>
/// 任务仓储
/// </summary>
public interface IJobInfoRepository : IRepository<JobInfo, Guid>
{
    /// <summary>
    /// 获取等待执行的分钟级任务
    /// 查询条件：启用且未废弃 + 分钟级别类型(Cron FixedRate) + 即将需要调度执行
    /// </summary>
    /// <param name="appId">应用ID</param>
    /// <returns>当前应用等待执行的分钟级任务列表</returns>
    Task<List<JobInfo>> GetWaitingMinuteJobsAsync(Guid appId);

    /// <summary>
    /// 获取还未分派给Worker的秒级任务
    /// </summary>
    /// <param name="appId">应用ID</param>
    /// <returns>当前应用等待分派的秒级任务列表</returns>
    Task<List<JobInfo>> GetWaitingSecondJobsAsync(Guid appId);
}
