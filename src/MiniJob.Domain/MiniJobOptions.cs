using MiniJob.Scheduler;
using Volo.Abp.Collections;

namespace MiniJob
{
    /// <summary>
    /// MiniJob配置选项
    /// </summary>
    public class MiniJobOptions
    {
        /// <summary>
        /// 任务调度调度周期，单位毫秒，默认为 15000
        /// </summary>
        public int AppSchedulePeriod { get; set; }

        /// <summary>
        /// 任务实例调度周期,单位毫秒,默认为 10000
        /// </summary>
        public int JobInstanceSchedulePeriod { get; set; }

        /// <summary>
        /// 任务实例清理周期,单位小时,默认为12小时
        /// </summary>
        public int CleanSchedulePeriod { get; set; }

        /// <summary>
        /// 任务实例保留天数,小于0表示永久保留,默认为3天
        /// </summary>
        public int JobInstanceRetention { get; set; }

        /// <summary>
        /// 状态存储组件名称,默认为 statestore
        /// </summary>
        public string StateName { get; set; }

        public ITypeList<IScheduler> Schedulers { get; set; }

        public MiniJobOptions()
        {
            AppSchedulePeriod = 15000;
            JobInstanceSchedulePeriod = 10000;
            CleanSchedulePeriod = 12;
            JobInstanceRetention = 3;
            StateName = "statestore";
            Schedulers = new TypeList<IScheduler>();
        }
    }
}
