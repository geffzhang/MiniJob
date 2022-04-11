namespace MiniJob.Dapr.Processes;

public interface IProcessFinder
{
    IEnumerable<IProcess> FindExistingProcesses(string processName);
}
