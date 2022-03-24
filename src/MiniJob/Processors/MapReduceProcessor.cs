namespace MiniJob.Processors;

/// <summary>
/// MapReduce执行处理器，适用于MapReduce任务
/// </summary>
public abstract class MapReduceProcessor : ProcessorBase
{
    /// <summary>
    /// 分发子任务
    /// </summary>
    /// <param name="taskList">子任务，再次执行时可通过 <see cref="ProcessorContext.SubTask"/> 获取</param>
    /// <param name="taskName">子任务名称，即子任务处理器中 <see cref="ProcessorContext.TaskName"/> 获取到的值</param>
    /// <returns></returns>
    /// <exception cref="MiniJobException">失败将抛出异常</exception>
    protected virtual async Task MapAsync(List<object> taskList, string taskName)
    {
        if (taskList.IsNullOrEmpty())
            return;

        await Task.CompletedTask;
    }

    /// <summary>
    /// 是否为根任务
    /// </summary>
    /// <returns>true -> 根任务 / false -> 非根任务</returns>
    /// <exception cref="NotImplementedException"></exception>
    protected virtual bool IsRootTask()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// ReduceAsync方法将在所有任务结束后调用
    /// </summary>
    /// <param name="context">任务上下文</param>
    /// <param name="taskResults">保存了各个子Task的执行结果</param>
    /// <returns>ReduceAsync产生的结果将作为任务最终的返回结果</returns>
    protected abstract Task<ProcessorResult> ReduceAsync(ProcessorContext context, List<TaskResult> taskResults);
}
