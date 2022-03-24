using Dapr.Actors.Runtime;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MiniJob.Dapr.Actors;
using MiniJob.Entities;
using MiniJob.Jobs;
using Volo.Abp.Domain.Repositories;

namespace MiniJob.Scheduler;

public class CleanDataActor : MiniJobActor, IRemindable, ICleanDataActor, IScheduler
{
    private const string ReminderName = "CleanDataReminder";

    protected MiniJobOptions MiniJobOptions { get; }

    protected IRepository<JobInstance, Guid> JobInstanceRepository { get; }

    public CleanDataActor(
        ActorHost host,
        IOptions<MiniJobOptions> options,
        IRepository<JobInstance, Guid> jobInstanceRepository)
        : base(host)
    {
        MiniJobOptions = options.Value;
        JobInstanceRepository = jobInstanceRepository;
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        Logger.LogInformation("start clean data scheduler.");
        await RegisterReminderAsync(ReminderName, null, TimeSpan.Zero, TimeSpan.FromHours(MiniJobOptions.CleanSchedulePeriod));
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        Logger.LogInformation("stop clean data scheduler.");
        await UnregisterReminderAsync(ReminderName);
    }

    public async Task ReceiveReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
    {
        try
        {
            await CleanSchedulerAsync();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "CleanScheduler error.");
        }
    }

    /// <summary>
    /// 清理数据调度
    /// </summary>
    /// <returns></returns>
    protected virtual async Task CleanSchedulerAsync()
    {
        // 删除数据库历史数据
        if (MiniJobOptions.JobInstanceRetention < 0)
            return;

        var beforeTime = Clock.Now - TimeSpan.FromDays(MiniJobOptions.JobInstanceRetention);
        var deletedInstance = await JobInstanceRepository
            .GetListAsync(p => p.LastModificationTime < beforeTime &&
                (p.InstanceStatus == InstanceStatus.Failed ||
                p.InstanceStatus == InstanceStatus.Succeed ||
                p.InstanceStatus == InstanceStatus.Stoped ||
                p.InstanceStatus == InstanceStatus.Canceled));
        await JobInstanceRepository.DeleteManyAsync(deletedInstance);

        Logger.LogInformation("deleted {Count} instanceInfo records whose modify time before {Time}.", deletedInstance.Count, beforeTime);
    }
}