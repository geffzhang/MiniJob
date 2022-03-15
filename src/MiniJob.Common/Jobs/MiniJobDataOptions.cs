using System.Collections.Immutable;

namespace MiniJob.Jobs;

public class MiniJobDataOptions
{
    private readonly Dictionary<Type, JobConfiguration> _jobConfigurationsByJobType;
    private readonly Dictionary<string, JobConfiguration> _jobConfigurationsByName;

    /// <summary>
    /// Default: true.
    /// </summary>
    public bool IsJobExecutionEnabled { get; set; } = true;

    public MiniJobDataOptions()
    {
        _jobConfigurationsByJobType = new Dictionary<Type, JobConfiguration>();
        _jobConfigurationsByName = new Dictionary<string, JobConfiguration>();
    }

    public JobConfiguration GetJob<TArgs>()
    {
        return GetJob(typeof(TArgs));
    }

    public JobConfiguration GetJob(Type jobType)
    {
        var jobConfiguration = _jobConfigurationsByJobType.TryGetValue(jobType, out var obj) ? obj : default;

        if (jobConfiguration == null)
        {
            throw new MiniJobException("Undefined job for the job type: " + jobType.AssemblyQualifiedName);
        }

        return jobConfiguration;
    }

    public JobConfiguration GetJob(string name)
    {
        var jobConfiguration = _jobConfigurationsByName.TryGetValue(name, out var obj) ? obj : default;

        if (jobConfiguration == null)
        {
            throw new MiniJobException("Undefined job for the job name: " + name);
        }

        return jobConfiguration;
    }

    public IReadOnlyList<JobConfiguration> GetJobs()
    {
        return _jobConfigurationsByJobType.Values.ToImmutableList();
    }

    public void AddJob<TJob>()
    {
        AddJob(typeof(TJob));
    }

    public void AddJob(Type jobType)
    {
        AddJob(new JobConfiguration(jobType));
    }

    public void AddJob(JobConfiguration jobConfiguration)
    {
        _jobConfigurationsByJobType[jobConfiguration.JobType] = jobConfiguration;
        _jobConfigurationsByName[jobConfiguration.JobName] = jobConfiguration;
    }
}