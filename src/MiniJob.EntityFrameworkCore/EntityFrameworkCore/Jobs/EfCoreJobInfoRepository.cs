using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MiniJob.Enums;
using MiniJob.Jobs;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Timing;

namespace MiniJob.EntityFrameworkCore.Jobs;

public class EfCoreJobInfoRepository : EfCoreRepository<MiniJobDbContext, JobInfo, Guid>, IJobInfoRepository
{
    protected IClock Clock { get; }
    protected MiniJobOptions MiniJobOptions { get; }

    public EfCoreJobInfoRepository(
        IDbContextProvider<MiniJobDbContext> dbContextProvider,
        IClock clock,
        IOptions<MiniJobOptions> options)
        : base(dbContextProvider)
    {
        Clock = clock;
        MiniJobOptions = options.Value;
    }

    public async Task<List<JobInfo>> GetWaitingMinuteJobsAsync(Guid appId)
    {
        return await (await GetDbContextAsync())
            .Set<JobInfo>()
            .AsNoTracking()
            .Where(p => p.AppId == appId && p.IsEnabled == true)
            .Where(p => p.TimeExpression == TimeExpressionType.Cron || 
                p.TimeExpression == TimeExpressionType.FixedRate || 
                p.TimeExpression == TimeExpressionType.Delayed)
            .Where(p => p.NextTriggerTime == null || p.NextTriggerTime <= Clock.Now.AddMilliseconds(MiniJobOptions.AppSchedulePeriod * 1.5))
            .Where(p => p.BeginTime == null || p.BeginTime >= Clock.Now)
            .Where(p => p.EndTime == null || p.EndTime <= Clock.Now)
            .OrderByDescending(t => t.JobPriority)
            .ThenBy(t => t.MaxTryCount)
            .ThenBy(t => t.NextTriggerTime)
            .ToListAsync();
    }

    public async Task<List<JobInfo>> GetWaitingSecondJobsAsync(Guid appId)
    {
        return await (await GetDbContextAsync())
            .Set<JobInfo>()
            .AsNoTracking()
            .Where(p => p.AppId == appId && p.IsEnabled == true)
            .Where(p => p.TimeExpression == TimeExpressionType.SecondDelay)
            .Where(p => !p.JobInstances.Any(x => x.JobInfoId == p.Id && (x.InstanceStatus == InstanceStatus.WaitingDispatch || x.InstanceStatus == InstanceStatus.WaitingWorkerReceive || x.InstanceStatus == InstanceStatus.Runing)))
            .Where(p => p.BeginTime == null || p.BeginTime >= Clock.Now)
            .Where(p => p.EndTime == null || p.EndTime <= Clock.Now)
            .OrderByDescending(t => t.JobPriority)
            .ThenBy(t => t.MaxTryCount)
            .ThenBy(t => t.NextTriggerTime)
            .ToListAsync();
    }
}
