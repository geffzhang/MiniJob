namespace MiniJob.Command;

public interface ICmdHelper
{
    void OpenWebPage(string url);

    void Run(string file, string arguments, bool waitForExit = true);

    string RunAndGetOutput(string file, string arguments, string workingDirectory = null, bool waitForExit = true);

    string RunAndGetOutput(string file, string arguments, out int exitCode, string workingDirectory = null, bool waitForExit = true);

    string RunAndGetOutput(string file, string arguments, out bool isExitCodeSuccessful, string workingDirectory = null, bool waitForExit = true);

    string GetArguments(string command);

    string GetFileName();

    void RunCmd(string command, string workingDirectory = null, bool waitForExit = true);

    void RunCmd(string command, out int exitCode, string workingDirectory = null, bool waitForExit = true);

    string RunCmdAndGetOutput(string command, string workingDirectory = null, bool waitForExit = true);

    string RunCmdAndGetOutput(string command, out bool isExitCodeSuccessful, string workingDirectory = null, bool waitForExit = true);

    string RunCmdAndGetOutput(string command, out int exitCode, string workingDirectory = null, bool waitForExit = true);
}
