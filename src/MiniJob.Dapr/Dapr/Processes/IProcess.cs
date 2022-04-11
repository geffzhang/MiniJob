namespace MiniJob.Dapr.Processes;

/// <summary>
/// 表示系统进程
/// </summary>
public interface IProcess
{
    /// <summary>
    /// 获取唯一进程标识符(PID)
    /// </summary>
    int? Id { get; }

    /// <summary>
    /// 获取进程名
    /// </summary>
    string Name { get; }

    /// <summary>
    /// 获取一个值，该值指示进程是否正在运行
    /// </summary>
    bool IsRunning { get; }

    /// <summary>
    /// 停止正在运行的进程
    /// </summary>
    /// <param name="waitForShutdownSeconds">An optional number of seconds to wait for graceful shutdown. If <c>null</c> the process will be terminated immediately.</param>
    /// <param name="cancellationToken">An optional cancellation token. When set this will terminate the process immediately if <paramref name="waitForShutdownSeconds"/> has not yet elapsed.</param>
    void Stop(int? waitForShutdownSeconds, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取初始化期间传递给进程的命令行参数
    /// </summary>
    /// <returns>A <see cref="IProcessCommandLine"/> instance.</returns>
    IProcessCommandLine GetCommandLine();
}
