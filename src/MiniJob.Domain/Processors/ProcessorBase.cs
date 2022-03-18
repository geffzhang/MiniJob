using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Diagnostics;
using Volo.Abp.DependencyInjection;

namespace MiniJob.Processors;

public abstract class ProcessorBase : IProcessor
{
    public IAbpLazyServiceProvider LazyServiceProvider { get; set; }

    protected ILoggerFactory LoggerFactory => LazyServiceProvider.LazyGetRequiredService<ILoggerFactory>();

    protected ILogger Logger => LazyServiceProvider.LazyGetService<ILogger>(provider => LoggerFactory?.CreateLogger(GetType().FullName) ?? NullLogger.Instance);

    public async Task<ProcessorResult> ExecuteAsync(ProcessorContext context)
    {
        Logger.LogInformation("using args: {Context}", context);

        string status = "unknown";
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        try
        {
            var result = await DoWorkAsync(context);
            Logger.LogInformation("execute succeed, using {Elapsed}, result: {ExecuteResult}", stopwatch.Elapsed, result);
            status = result.Success ? "success" : "failed";
            return result;
        }
        catch (Exception ex)
        {
            status = "exception";
            Logger.LogError(ex, "execute failed!");
            return ProcessorResult.ErrorMessage(ex.Message);
        }
        finally
        {
            stopwatch.Stop();
            Logger.LogInformation("{JobInfoId} JobInstanceId-{JobInstanceId}|{Status}|{Elapsed}", context.JobId, context.JobInstanceId, status, stopwatch.Elapsed);
            await DisposeAsync();
        }
    }

    public virtual ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// 核心处理逻辑
    /// </summary>
    /// <param name="context">任务上下文，可通过 JobArgs 和 InstanceArgs 分别获取控制台参数和OpenAPI传递的任务实例参数</param>
    /// <returns>处理结果，Message有长度限制，超长会被裁剪，不允许返回 null</returns>
    protected abstract Task<ProcessorResult> DoWorkAsync(ProcessorContext context);
}
