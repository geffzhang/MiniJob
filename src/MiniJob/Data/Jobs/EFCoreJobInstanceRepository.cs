using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MiniJob.Data;
using MiniJob.Entities;
using MiniJob.Entities.Jobs;
using MiniJob.Scheduler;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Timing;

namespace MiniJob.EntityFrameworkCore.Jobs;

public class EFCoreJobInstanceRepository : EfCoreRepository<MiniJobDbContext, JobInstance, Guid>, IJobInstanceRepository
{
    protected IClock Clock { get; }
    protected MiniJobSchedulerOptions MiniJobOptions { get; }

    public EFCoreJobInstanceRepository(
        IDbContextProvider<MiniJobDbContext> dbContextProvider,
        IClock clock,
        IOptions<MiniJobSchedulerOptions> options)
        : base(dbContextProvider)
    {
        Clock = clock;
        MiniJobOptions = options.Value;
    }

    public async Task<List<JobInstance>> GetRuningSecondJobInstanceAsync(Guid jobId)
    {
        return await (await GetDbContextAsync())
            .Set<JobInstance>()
            .AsNoTracking()
            .Where(p => p.JobInfoId == jobId)
            .Where(p => p.InstanceStatus == InstanceStatus.WaitingDispatch ||
                        p.InstanceStatus == InstanceStatus.WaitingWorkerReceive ||
                        p.InstanceStatus == InstanceStatus.Runing)
            .ToListAsync();
    }
}
