using Dapr.Actors.Runtime;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MiniJob.Dapr.Actors;
using MiniJob.Enums;
using MiniJob.Jobs;
using System.Diagnostics;
using Volo.Abp.DistributedLocking;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;

namespace MiniJob.Scheduler;

public class JobTrackerActor : MiniJobActor, IJobTrackerActor
{
    protected IAbpDistributedLock DistributedLock { get; }
    protected IJobInfoRepository JobInfoRepository { get; }
    protected IRepository<JobInstance, Guid> JobInstanceRepository { get; }
    protected MiniJobOptions MiniJobOptions { get; }

    public JobTrackerActor(
        ActorHost host,
        IAbpDistributedLock distributedLock,
        IJobInfoRepository jobInfoRepository,
        IRepository<JobInstance, Guid> jobInstanceRepository,
        IOptions<MiniJobOptions> options)
        : base(host)
    {
        DistributedLock = distributedLock;
        JobInfoRepository = jobInfoRepository;
        JobInstanceRepository = jobInstanceRepository;
        MiniJobOptions = options.Value;
    }

    [UnitOfWork]
    public async Task TrackAsync()
    {
        var appId = Guid.Parse(Id.GetId());
        var start = Clock.Now;
        Stopwatch stopwatch = new();
        stopwatch.Start();

        try
        {
            await ScheduleMinuteJobAsync(appId);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "app-{AppId} schedule minute job failed.", appId);
        }

        var minuteTime = stopwatch.ElapsedMilliseconds;
        stopwatch.Reset();
        stopwatch.Start();

        try
        {
            await ScheduleSecondJobAsync(appId);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "app-{AppId} schedule second job failed.", appId);
        }

        Logger.LogInformation("app-{AppId} minute schedule: {MinuteTime}ms, second schedule: {SecondTime}ms", appId, minuteTime, stopwatch.ElapsedMilliseconds);

        stopwatch.Stop();

        var cost = Clock.Now - start;
        if (Clock.Now - start > TimeSpan.FromMilliseconds(MiniJobOptions.AppSchedulePeriod))
        {
            Logger.LogWarning("app-{AppId} The database query is using too much time({Cost}), please check if the database load is too high!", appId, cost);
        }
    }

    /// <summary>
    /// 分钟级别任务调度
    /// </summary>
    /// <remarks>
    /// <para>1、获取应用所有等待执行的分钟级任务(2 * <see cref="MiniJobOptions.AppSchedulePeriod"/>内需要调度执行的任务)</para>
    /// <para>2、创建任务实例，并激活派送Actor<see cref="IJobDispatchActor"/>，到执行时间后派送到Worker执行</para>
    /// <para>3、计算任务下次调度时间</para>
    /// </remarks>
    /// <param name="appId">应用ID</param>
    /// <returns></returns>
    protected virtual async Task ScheduleMinuteJobAsync(Guid appId)
    {
        var jobInfos = await JobInfoRepository.GetWaitingMinuteJobsAsync(appId);
        if (!jobInfos.Any())
            return;

        Logger.LogInformation("app-{AppId} These {Count} minute jobs will be scheduled: {JobInfos}.", appId, jobInfos.Count, jobInfos);

        foreach (var jobInfo in jobInfos)
        {
            // 如果没有下次触发时间则先计算
            if (!jobInfo.NextTriggerTime.HasValue)
                jobInfo.CalculateNextTriggerTime(Clock.Now);
            if (jobInfo.NextTriggerTime > Clock.Now.AddMilliseconds(MiniJobOptions.AppSchedulePeriod * 1.5))
                continue;

            // 计算任务实例到期时间
            TimeSpan dueTime = TimeSpan.Zero;
            if (jobInfo.NextTriggerTime < Clock.Now)
            {
                Logger.LogWarning("{JobInfo} schedule delay, expect: {NextTriggerTime}, current: {Now}",
                    jobInfo, jobInfo.NextTriggerTime, Clock.Now);

                // 延迟任务过期后立即触发一次
                if (jobInfo.MisfireStrategy == MisfireStrategy.Ignore && jobInfo.TimeExpression != TimeExpressionType.Delayed)
                {
                    await UpdateJobNextTriggerTime(jobInfo);

                    Logger.LogInformation("MisfireStrategy is {MisfireStrategy}, continue this job, next trigger time is {NextTriggerTime}", jobInfo.MisfireStrategy, jobInfo.NextTriggerTime);
                    continue;
                }
            }
            else
            {
                dueTime = jobInfo.NextTriggerTime.Value - Clock.Now;
            }

            // 保存任务实例
            var jobInstance = new JobInstance(GuidGenerator.Create(), jobInfo.AppId, jobInfo.Id, jobInfo.NextTriggerTime.Value);
            await JobInstanceRepository.InsertAsync(jobInstance);

            await UpdateJobNextTriggerTime(jobInfo);

            // 激活派发Actor，到期后派送任务实例到Worker
            await ActorHelper.CreateActor<IJobDispatchActor, JobDispatchActor>(jobInstance.Id.ToString())
                .DispatchAsync(dueTime);
        }
    }

    protected virtual async Task UpdateJobNextTriggerTime(JobInfo jobInfo)
    {
        CalculateJobInfoNextTriggerTime(jobInfo);

        await JobInfoRepository.UpdateAsync(jobInfo);
    }

    /// <summary>
    /// 秒级别任务调度
    /// </summary>
    /// <returns></returns>
    protected virtual async Task ScheduleSecondJobAsync(Guid appId)
    {
        var jobInfos = await JobInfoRepository.GetWaitingSecondJobsAsync(appId);
        if (!jobInfos.Any())
            return;

        Logger.LogInformation("app-{AppId} These {Count} second jobs will be scheduled： {JofInfo}.", appId, jobInfos.Count, jobInfos);

        foreach (var jobInfo in jobInfos)
        {
            try
            {
                //await JobService.RunJobAsync(jobInfo.AppId, jobInfo.Id, null, TimeSpan.Zero);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "app-{AppId}|{JobInfo} schedule second job failed.", appId, jobInfo);
            }
        }
    }

    /// <summary>
    /// 计算任务下次触发时间
    /// </summary>
    /// <param name="jobInfo"></param>
    protected virtual void CalculateJobInfoNextTriggerTime(JobInfo jobInfo)
    {
        // 计算任务下一次调度时间
        try
        {
            // 延迟任务只执行一次
            if (jobInfo.TimeExpression != TimeExpressionType.Delayed)
            {
                // 取最大值,防止长时间未调度任务被连续调度
                var maxTime = jobInfo.NextTriggerTime.Value > Clock.Now ?
                    jobInfo.NextTriggerTime :
                    Clock.Now;

                jobInfo.CalculateNextTriggerTime(maxTime.Value);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "{JobInfo} refresh job failed, system will set job to DISABLE!", jobInfo);
            jobInfo.IsEnabled = false;
        }
    }
}
