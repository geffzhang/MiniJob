using System.Runtime.InteropServices;

namespace MiniJob.Processors;

/// <summary>
/// Shell 处理器
/// </summary>
public class ShellProcessor : ScriptProcessorBase
{
    protected override string GetFileName()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            //Windows
            return "cmd.exe";
        }

        //Linux or OSX
        if (File.Exists("/bin/bash"))
        {
            return "/bin/bash";
        }

        if (File.Exists("/bin/sh"))
        {
            return "/bin/sh"; //some Linux distributions like Alpine doesn't have bash
        }

        throw new MiniJobException($"Cannot determine shell command for this OS! " +
                               $"Running on OS: {RuntimeInformation.OSDescription} | " +
                               $"OS Architecture: {RuntimeInformation.OSArchitecture} | " +
                               $"Framework: {RuntimeInformation.FrameworkDescription} | " +
                               $"Process Architecture{RuntimeInformation.ProcessArchitecture}");
    }

    protected override string GetScriptName(Guid jobInstanceId)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return $"shell_{jobInstanceId}.bat";
        }

        return $"shell_{jobInstanceId}.sh";
    }
}
