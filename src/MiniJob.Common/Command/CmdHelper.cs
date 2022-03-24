using System.Diagnostics;
using System.Runtime.InteropServices;
using Volo.Abp.DependencyInjection;

namespace MiniJob.Command;

public class CmdHelper : ICmdHelper, ITransientDependency
{
    private const int SuccessfulExitCode = 0;

    public void OpenWebPage(string url)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            url = url.Replace("&", "^&");
            Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            Process.Start("xdg-open", url);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            Process.Start("open", url);
        }
    }

    public void Run(string file, string arguments, bool waitForExit = true)
    {
        var procStartInfo = new ProcessStartInfo(file, arguments);
        Process.Start(procStartInfo)?.WaitForExit(waitForExit);
    }

    public void RunCmd(string command, string workingDirectory = null, bool waitForExit = true)
    {
        RunCmd(command, out _, workingDirectory, waitForExit);
    }

    public void RunCmd(string command, out int exitCode, string workingDirectory = null, bool waitForExit = true)
    {
        var procStartInfo = new ProcessStartInfo(
            GetFileName(),
            GetArguments(command)
        );

        if (!string.IsNullOrEmpty(workingDirectory))
        {
            procStartInfo.WorkingDirectory = workingDirectory;
        }

        using (var process = Process.Start(procStartInfo))
        {
            process?.WaitForExit(waitForExit);

            exitCode = 0;

            // 等待进程退出才能获取退出码
            if (waitForExit)
                exitCode = process.ExitCode;
        }
    }

    public string RunCmdAndGetOutput(string command, string workingDirectory = null, bool waitForExit = true)
    {
        return RunCmdAndGetOutput(command, out int _, workingDirectory, waitForExit);
    }

    public string RunCmdAndGetOutput(string command, out bool isExitCodeSuccessful, string workingDirectory = null, bool waitForExit = true)
    {
        var output = RunCmdAndGetOutput(command, out int exitCode, workingDirectory, waitForExit);
        isExitCodeSuccessful = exitCode == SuccessfulExitCode;
        return output;
    }

    public string RunCmdAndGetOutput(string command, out int exitCode, string workingDirectory = null, bool waitForExit = true)
    {
        return RunAndGetOutput(GetFileName(), GetArguments(command), out exitCode, workingDirectory, waitForExit);
    }

    public string RunAndGetOutput(string file, string arguments, string workingDirectory = null, bool waitForExit = true)
    {
        return RunAndGetOutput(file, arguments, out int _, workingDirectory, waitForExit);
    }

    public string RunAndGetOutput(string file, string arguments, out bool isExitCodeSuccessful, string workingDirectory = null, bool waitForExit = true)
    {
        var output = RunAndGetOutput(file, arguments, out int exitCode, workingDirectory, waitForExit);
        isExitCodeSuccessful = exitCode == SuccessfulExitCode;
        return output;
    }

    public string RunAndGetOutput(string file, string arguments, out int exitCode, string workingDirectory = null, bool waitForExit = true)
    {
        string output;

        using (var process = new Process())
        {
            process.StartInfo = new ProcessStartInfo(file)
            {
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            if (!string.IsNullOrEmpty(workingDirectory))
            {
                process.StartInfo.WorkingDirectory = workingDirectory;
            }

            process.Start();

            using (var standardOutput = process.StandardOutput)
            {
                using (var standardError = process.StandardError)
                {
                    output = standardOutput.ReadToEnd();
                    output += standardError.ReadToEnd();
                }
            }

            process.WaitForExit(waitForExit);

            exitCode = 0;

            // 等待进程退出才能获取退出码
            if (waitForExit)
                exitCode = process.ExitCode;
        }

        return output.Trim();
    }

    public string GetArguments(string command)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return "-c \"" + command + "\"";
        }

        //Windows default.
        return "/C \"" + command + "\"";
    }

    public string GetFileName()
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

        throw new Exception($"Cannot determine shell command for this OS! " +
                            $"Running on OS: {RuntimeInformation.OSDescription} | " +
                            $"OS Architecture: {RuntimeInformation.OSArchitecture} | " +
                            $"Framework: {RuntimeInformation.FrameworkDescription} | " +
                            $"Process Architecture{RuntimeInformation.ProcessArchitecture}");
    }
}
