namespace MiniJob.Jobs;

public interface IJob : IDisposable
{
    Task<JobResult> ExecuteAsync(JobContext context);
}