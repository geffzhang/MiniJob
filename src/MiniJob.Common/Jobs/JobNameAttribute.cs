namespace MiniJob.Jobs;

public class JobNameAttribute : Attribute, IJobNameProvider
{
    public string Name { get; }

    public JobNameAttribute(string name)
    {
        Name = name;
    }

    public static string GetName<TJobArgs>()
    {
        return GetName(typeof(TJobArgs));
    }

    public static string GetName(Type jobType)
    {
        return jobType
                   .GetCustomAttributes(true)
                   .OfType<IJobNameProvider>()
                   .FirstOrDefault()
                   ?.Name
               ?? jobType.FullName;
    }
}