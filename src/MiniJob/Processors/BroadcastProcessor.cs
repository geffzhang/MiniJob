namespace MiniJob.Processors;

/// <summary>
/// 广播执行处理器，适用于广播执行
/// </summary>
public abstract class BroadcastProcessor : ProcessorBase
{
    /// <summary>
    /// 在所有节点广播执行前执行，只会在一台机器执行一次
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    protected virtual Task<ProcessorResult> PreDoWorkAsync(ProcessorContext context)
    {
        return Task.FromResult(ProcessorResult.OK);
    }

    /// <summary>
    /// 在所有节点广播执行完成后执行，只会在一台机器执行一次
    /// </summary>
    /// <param name="context"></param>
    /// <param name="taskResults"></param>
    /// <returns></returns>
    protected virtual Task<ProcessorResult> PostDoWorkAsync(ProcessorContext context, List<TaskResult> taskResults)
    {
        return Task.FromResult(DefaultPostResult(taskResults));
    }

    /// <summary>
    /// 默认的所有节点广播执行完成后的结果
    /// </summary>
    /// <param name="taskResults"></param>
    /// <returns></returns>
    private static ProcessorResult DefaultPostResult(List<TaskResult> taskResults)
    {
        var failedCount = taskResults.Count(p => !p.Success);
        var succeedCount = taskResults.Count(p => p.Success);

        return new ProcessorResult(failedCount == 0, $"succeed:{succeedCount}, failed:{failedCount}");
    }
}
