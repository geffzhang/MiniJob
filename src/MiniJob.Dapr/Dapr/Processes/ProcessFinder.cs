using System.Diagnostics;

namespace MiniJob.Dapr.Processes;

internal class ProcessFinder : IProcessFinder
{
    public IEnumerable<IProcess> FindExistingProcesses(string processName) =>
        Process.GetProcessesByName(processName).Select(x => (IProcess)new SystemProcess(x));
}
