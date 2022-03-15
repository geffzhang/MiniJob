namespace MiniJob.Jobs;

public interface IJobNameProvider
{
    string Name { get; }
}