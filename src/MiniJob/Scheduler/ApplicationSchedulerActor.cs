using Dapr.Actors.Runtime;
using Microsoft.Extensions.Options;
using MiniJob.Dapr.Actors;
using MiniJob.Entities.Jobs;
using Volo.Abp.Domain.Repositories;

namespace MiniJob.Scheduler;

public class ApplicationSchedulerActor : MiniJobActor, IApplicationSchedulerActor, IRemindable
{
    private const string SchedulerReminderName = "GlobalAppJobSchedulerReminder";

    protected MiniJobOptions MiniJobOptions { get; }
    protected IRepository<AppInfo, Guid> AppInfoRepository { get; }

    public ApplicationSchedulerActor(
        ActorHost host,
        IOptions<MiniJobOptions> options,
        IRepository<AppInfo, Guid> appInfoRepository)
        : base(host)
    {
        MiniJobOptions = options.Value;
        AppInfoRepository = appInfoRepository;
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        // 注册任务调度提醒
        await RegisterReminderAsync(SchedulerReminderName, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(MiniJobOptions.AppSchedulePeriod));
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        await UnregisterReminderAsync(SchedulerReminderName);
    }

    public async Task ReceiveReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
    {
        try
        {
            await AppSchedulerAsync();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "application scheduler failed");
        }
    }

    /// <summary>
    /// 发起任务调度
    /// </summary>
    /// <returns></returns>
    protected virtual async Task AppSchedulerAsync()
    {
        var enableAppInfos = await AppInfoRepository.GetListAsync(p => p.IsEnabled);
        foreach (var appInfo in enableAppInfos)
        {
            var jobTracker = ActorHelper.CreateActor<IJobTrackerActor, JobTrackerActor>(appInfo.Id.ToString());
            await jobTracker.TrackAsync();
        }
    }
}
