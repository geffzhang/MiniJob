using Microsoft.Extensions.Logging;
using MiniJob.Helpers;
using System.Runtime.InteropServices;

namespace MiniJob.Processors;

/// <summary>
/// 脚本处理器
/// </summary>
public abstract class ScriptProcessorBase : ProcessorBase
{
    /// <summary>
    /// 获取运行命令（eg，Linux shell返回 /bin/sh，Windows返回 cmd.exe）
    /// </summary>
    /// <returns>执行脚本的命令</returns>
    protected abstract string GetFileName();

    /// <summary>
    /// 生成脚本名称
    /// </summary>
    /// <param name="jobInstanceId">任务实例ID</param>
    /// <returns>文件名称</returns>
    protected abstract string GetScriptName(Guid jobInstanceId);

    protected override async Task<ProcessorResult> DoWorkAsync(ProcessorContext context)
    {
        var scriptArgs = context.GetArgs();

        if (scriptArgs.IsNullOrEmpty())
        {
            string message = "script args is null, please check job args configuration.";
            Logger.LogWarning(message);
            return ProcessorResult.ErrorMessage(message);
        }

        Logger.LogInformation("ScriptProcessor start to execute, args: {Args}", scriptArgs);

        var scriptPath = await PrepareScriptFile(context.JobInstanceId, scriptArgs);
        Logger.LogInformation("generate executable file successfully, path: {Path}", scriptPath);

        var cmdHelper = LazyServiceProvider.LazyGetRequiredService<ICmdHelper>();

        string result = cmdHelper.RunAndGetOutput(GetFileName(), GetArguments(scriptPath), out int exitCode);

        return new ProcessorResult(exitCode == 0, result);
    }

    /// <summary>
    /// 准备脚本文件
    /// </summary>
    /// <param name="jobInstanceId"></param>
    /// <param name="scriptArgs"></param>
    /// <returns></returns>
    protected virtual async Task<string> PrepareScriptFile(Guid jobInstanceId, string scriptArgs)
    {
        if (!Directory.Exists(MiniJobConsts.ScriptWorkerDir))
            Directory.CreateDirectory(MiniJobConsts.ScriptWorkerDir);

        // todo: 清理脚本文件
        var scriptPath = Path.Combine(MiniJobConsts.ScriptWorkerDir, GetScriptName(jobInstanceId));

        // 如果是下载链接，则从网络获取
        foreach (var protocol in MiniJobConsts.DownloadProtocol)
        {
            if (scriptArgs.StartsWith(protocol))
            {
                var httpClientFactory = LazyServiceProvider.LazyGetRequiredService<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient();
                httpClient.Timeout = TimeSpan.FromSeconds(300);
                scriptArgs = await httpClient.GetStringAsync(scriptArgs);
            }
        }

        // 非下载链接，为 processInfo 生成可执行文件
        await File.WriteAllTextAsync(scriptPath, scriptArgs);

        return scriptPath;
    }

    protected virtual string GetArguments(string command)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return "-c \"" + command + "\"";
        }

        //Windows default.
        return "/C \"" + command + "\"";
    }
}
