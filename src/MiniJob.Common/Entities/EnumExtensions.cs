namespace MiniJob.Entities;

public static class EnumExtensions
{
    /// <summary>
    /// 任务实例是否结束状态
    /// </summary>
    /// <param name="jobInstanceStatus"></param>
    /// <returns></returns>
    public static bool IsFinished(this InstanceStatus jobInstanceStatus)
    {
        return jobInstanceStatus == InstanceStatus.Failed ||
            jobInstanceStatus == InstanceStatus.Succeed ||
            jobInstanceStatus == InstanceStatus.Canceled ||
            jobInstanceStatus == InstanceStatus.Stoped;
    }

    /// <summary>
    /// 秒级任务是否正在运行中
    /// </summary>
    /// <param name="jobInstanceStatus"></param>
    /// <returns></returns>
    public static bool IsRunning(this InstanceStatus jobInstanceStatus)
    {
        return jobInstanceStatus == InstanceStatus.WaitingDispatch ||
            jobInstanceStatus == InstanceStatus.WaitingWorkerReceive ||
            jobInstanceStatus == InstanceStatus.Runing;
    }

    /// <summary>
    /// 是否秒级任务
    /// </summary>
    /// <param name="timeExpression"></param>
    /// <returns></returns>
    public static bool IsSecondType(this TimeExpressionType timeExpression)
    {
        return timeExpression == TimeExpressionType.SecondDelay;
    }

    /// <summary>
    /// 是否分钟级任务
    /// </summary>
    /// <param name="timeExpression"></param>
    /// <returns></returns>
    public static bool IsMinuteType(this TimeExpressionType timeExpression)
    {
        return timeExpression == TimeExpressionType.Cron || timeExpression == TimeExpressionType.FixedRate;
    }
}