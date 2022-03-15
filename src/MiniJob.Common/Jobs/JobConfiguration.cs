namespace MiniJob.Jobs;

public class JobConfiguration
{
    public Type JobType { get; }

    public string JobName { get; }

    public JobConfiguration(Type jobType)
    {
        JobType = jobType;
        JobName = JobNameAttribute.GetName(jobType);
    }
}