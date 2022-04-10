namespace MiniJob.Dapr.Process;

public interface IDaprProcessHost<TOptions> : IDaprProcessHost
    where TOptions : Options.DaprProcessOptions
{
    /// <summary>
    /// Gets the process options used to configure and start up the current running process.
    /// </summary>
    /// <returns>A <typeparamref name="TOptions"/> instance, or <c>null</c> if the process is not currently running.</returns>
    new TOptions GetProcessOptions();
}

/// <summary>
/// 表示一个 Dapr 进程主机
/// </summary>
public interface IDaprProcessHost
{
    /// <summary>
    /// 获取用于配置和启动当前正在运行的进程的进程配置选项
    /// </summary>
    /// <returns>一个 <see cref="Options.DaprProcessOptions"/> 实例, 如果不是当前运行的进程则为空</returns>
    Options.DaprProcessOptions GetProcessOptions();

    /// <summary>
    /// 获取有关当前运行的进程的信息
    /// <para>如果不是当前运行的进程则返回 <see cref="DaprProcessInfo.Unknown"/></para>
    /// </summary>
    /// <returns>一个 <see cref="DaprProcessInfo"/> 实例.</returns>
    DaprProcessInfo GetProcessInfo();

    bool Start(Func<DaprOptions> optionsAccessor, CancellationToken cancellationToken);

    void Stop(CancellationToken cancellationToken);

    /// <summary>
    /// 获取Dapr进程的当前运行状况
    /// </summary>
    /// <param name="cancellationToken">A token for cancelling the operation.</param>
    /// <returns>A <see cref="DaprHealthResult"/>.</returns>
    Task<DaprHealthResult> GetHealthAsync(CancellationToken cancellationToken);

    /// <summary>
    /// 从Dapr进程中检索指标并写入所提供的 <paramref name="stream"/>.
    /// </summary>
    /// <param name="stream">指标将被写入的可写流</param>
    /// <param name="cancellationToken">A token for cancelling the operation.</param>
    /// <returns>写入流的字节数</returns>
    Task<int> WriteMetricsAsync(Stream stream, CancellationToken cancellationToken);
}
