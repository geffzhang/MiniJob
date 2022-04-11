namespace MiniJob.Dapr.Processes;

/// <summary>
/// 检查操作系统端口的可用性
/// </summary>
public interface IPortAvailabilityChecker
{
    /// <summary>
    /// Gets the next available port starting from <paramref name="startingPort"/>
    /// excluding any defined in <paramref name="reservedPorts"/>.
    /// </summary>
    /// <param name="startingPort">The preferred starting port. If available this value will be returned, else it will be the next available incremental value above it.</param>
    /// <param name="reservedPorts">An optional list of reserved ports. If defined the returned value will not be one of the ports in this list.</param>
    /// <returns>The next available port.</returns>
    int GetAvailablePort(int startingPort, IEnumerable<int> reservedPorts = null);
}
