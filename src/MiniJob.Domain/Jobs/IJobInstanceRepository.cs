using Volo.Abp.Domain.Repositories;

namespace MiniJob.Jobs;

/// <summary>
/// 任务实例仓储
/// </summary>
public interface IJobInstanceRepository : IRepository<JobInstance, Guid>
{
    /// <summary>
    /// 获取运行中的秒级任务实例
    /// </summary>
    /// <param name="jobId">任务ID</param>
    /// <returns>运行中的秒级任务实例</returns>
    Task<List<JobInstance>> GetRuningSecondJobInstanceAsync(Guid jobId);
}
