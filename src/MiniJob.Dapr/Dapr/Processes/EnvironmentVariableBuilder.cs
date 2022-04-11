namespace MiniJob.Dapr.Processes;

public class EnvironmentVariableBuilder
{
    private readonly Dictionary<string, object> _values = new Dictionary<string, object>();

    public EnvironmentVariableBuilder Add(string name, object value, Func<bool> predicate = null)
    {
        // Precondition checks
        if (name.IsNullOrWhiteSpace() || value == null || predicate?.Invoke() == false)
        {
            return this;
        }

        // Add or replace
        _values[name] = value;

        return this;
    }

    public IDictionary<string, object> ToDictionary() => new Dictionary<string, object>(_values);
}
