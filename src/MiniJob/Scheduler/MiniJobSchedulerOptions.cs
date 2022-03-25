using Volo.Abp.Collections;

namespace MiniJob.Scheduler;

/// <summary>
/// MiniJob配置选项
/// </summary>
public class MiniJobSchedulerOptions
{
    /// <summary>
    /// 任务调度调度周期，单位毫秒，默认为 15000
    /// </summary>
    public int AppSchedulePeriod { get; set; }

    /// <summary>
    /// 任务实例清理周期,单位小时,默认为12小时
    /// </summary>
    public int CleanSchedulePeriod { get; set; }

    /// <summary>
    /// 任务实例保留天数,小于0表示永久保留,默认为3天
    /// </summary>
    public int JobInstanceRetention { get; set; }

    public ITypeList<IScheduler> Schedulers { get; set; }

    public MiniJobSchedulerOptions()
    {
        AppSchedulePeriod = 15000;
        CleanSchedulePeriod = 12;
        JobInstanceRetention = 3;
        Schedulers = new TypeList<IScheduler>();
    }
}