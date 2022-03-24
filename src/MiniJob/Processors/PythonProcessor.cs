namespace MiniJob.Processors;

/// <summary>
/// Python 处理器
/// </summary>
public class PythonProcessor : ScriptProcessorBase
{
    protected override string GetFileName()
    {
        return "python";
    }

    protected override string GetScriptName(Guid jobInstanceId)
    {
        return $"python_{jobInstanceId}.py";
    }

    protected override string GetArguments(string command)
    {
        return command;
    }
}
