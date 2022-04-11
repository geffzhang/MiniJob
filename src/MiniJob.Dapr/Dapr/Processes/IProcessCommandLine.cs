namespace MiniJob.Dapr.Processes;

/// <summary>
/// 表示用于启动进程的命令行
/// </summary>
public interface IProcessCommandLine
{
    /// <summary>
    /// 获取使用此命令行启动的进程
    /// </summary>
    IProcess Process { get; }

    /// <summary>
    /// 获取完整的命令行文本
    /// </summary>
    string CommandLine { get; }

    /// <summary>
    /// Gets all command-line arguments as a dictionary of name-value pairs separated by the specified <paramref name="separator"/>.
    /// </summary>
    /// <param name="separator">The argument separator or prefix.</param>
    /// <returns>A dictionary of command-line arguments.</returns>
    IDictionary<string, string> GetArgumentsAsDictionary(char separator);
}
