namespace MiniJob.Processors;

/// <summary>
/// Go 处理器
/// </summary>
public class GoProcessor : ScriptProcessorBase
{
    protected override string GetFileName()
    {
        return "go";
    }

    protected override string GetScriptName(Guid jobInstanceId)
    {
        return $"go_{jobInstanceId}.go";
    }

    protected override string GetArguments(string command)
    {
        return command;
    }
}
