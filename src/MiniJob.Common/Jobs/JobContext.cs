using MiniJob.Enums;

namespace MiniJob.Jobs;

public class JobContext
{
    public virtual Guid JobId { get; set; }

    public virtual Guid JobInstanceId { get; set; }

    public virtual Guid? SubJobInstanceId { get; set; }

    public virtual Guid TaskId { get; set; }

    public virtual string TaskName { get; set; }

    /// <summary>
    /// 通过控制台传递的参数
    /// </summary>
    public virtual string JobArgs { get; set; }

    /// <summary>
    /// 任务实例运行中参数
    /// 若该任务实例通过 OpenAPI 触发，则该值为 OpenAPI 传递的参数
    /// 若该任务为工作流的某个节点，则该值为工作流实例的上下文 ( wfContext )
    /// </summary>
    public virtual string InstanceArgs { get; set; }

    /// <summary>
    /// 最大重试次数
    /// </summary>
    public virtual int MaxTryCount { get; set; }

    /// <summary>
    /// 当前重试次数
    /// </summary>
    public virtual int TryCount { get; set; }

    /// <summary>
    /// 子任务对象，通过Map/MapReduce处理器的map方法生成
    /// </summary>
    public virtual object SubTask { get; set; }

    /// <summary>
    /// 处理器信息(类型全称)
    /// </summary>
    public virtual string ExecutorInfo { get; set; }

    public virtual ProcessorType ProcessorType { get; set; }

    /// <summary>
    /// 获取参数
    /// </summary>
    /// <returns></returns>
    public virtual string GetArgs()
    {
        if (!string.IsNullOrWhiteSpace(InstanceArgs))
            return InstanceArgs;
        return JobArgs;
    }

    public override string ToString()
    {
        return $"JobId = {JobId}, " +
               $"JobInstanceId = {JobInstanceId}, " +
               $"TaskId = {TaskId}, " +
               $"TaskName = {TaskName}, " +
               $"TryCount = {TryCount}, " +
               $"JobArgs = {JobArgs}, " +
               $"InstanceArgs = {InstanceArgs}";
    }
}
