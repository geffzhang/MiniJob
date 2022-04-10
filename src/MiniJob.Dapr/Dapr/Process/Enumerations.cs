namespace MiniJob.Dapr.Process;

/// <summary>
/// Dapr进程状态
/// </summary>
public enum DaprProcessStatus
{
    Stopped = 0,
    Initializing = 1,
    Starting = 2,
    Started = 3,
    Stopping = 4,
    Disabled = 5
}

internal enum ProcessComparison
{
    None = 0,
    Duplicate = 1,
    Attachable = 2
}
