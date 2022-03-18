namespace System.Diagnostics;

public static class ProcessExtensions
{
    public static Process WaitForExit(this Process process, bool waitForExit)
    {
        if (waitForExit)
            process.WaitForExit();

        return process;
    }

    public static async Task<Process> WaitForExitAsync(this Process process, bool waitForExit)
    {
        if (waitForExit)
            await process.WaitForExitAsync();

        return process;
    }
}
