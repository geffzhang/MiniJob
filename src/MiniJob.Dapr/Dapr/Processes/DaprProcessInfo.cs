﻿namespace MiniJob.Dapr.Processes;

/// <summary>
/// Dapr进程信息
/// </summary>
public class DaprProcessInfo
{
    public static DaprProcessInfo Unknown => new DaprProcessInfo("unknown", null, null, DaprProcessStatus.Stopped, false);

    public DaprProcessInfo(string name, int? id, string version, DaprProcessStatus status)
        : this(name, id, version, status, false)
    {
    }

    public DaprProcessInfo(string name, int? id, string version, DaprProcessStatus status, bool isAttached)
    {
        Name = name;
        Id = id;
        Version = version;
        Status = status;
        IsAttached = isAttached;
    }

    public string Name { get; }

    public int? Id { get; }

    public string Version { get; }

    public DaprProcessStatus Status { get; }

    public bool IsRunning => Status == DaprProcessStatus.Started;

    public bool IsAttached { get; }

    public string Description =>
        IsRunning ? (!Version.IsNullOrEmpty() ? $"Dapr process '{Name}' running, version {Version}" : $"Dapr process '{Name}' running, unverified version") :
        IsAttached ? (!Version.IsNullOrEmpty() ? $"Dapr process '{Name}' attached, version {Version}" : $"Dapr process '{Name}' attached, unverified version") :
        $"Dapr process '{Name}' not available, status is {Status}";

    public override string ToString() => Description;
}
