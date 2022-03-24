using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;

namespace MiniJob.Processors;

/// <summary>
/// 文件清理处理器
/// </summary>
public class FileCleanupProcessor : BroadcastProcessor
{
    protected override Task<ProcessorResult> DoWorkAsync(ProcessorContext context)
    {
        Logger.LogInformation("using args: {Args}", context.JobArgs);

        int cleanCount = 0;
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var cleanupArgs = JsonSerializer.Deserialize<List<FileCleanupArgs>>(context.JobArgs);
        foreach (var cleanupArg in cleanupArgs)
        {
            Logger.LogInformation("start to process: {@CleanupArgs}", cleanupArg);

            if (cleanupArg.SearchPattern.IsNullOrEmpty() || cleanupArg.DirPath.IsNullOrEmpty())
            {
                Logger.LogWarning("skip due to invalid args!");
                continue;
            }

            if (!Directory.Exists(cleanupArg.DirPath))
            {
                Logger.LogWarning("skip due to dirpath[{Path}] not exists", cleanupArg.DirPath);
                continue;
            }

            var searchOption = cleanupArg.IncludeSubDir ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            var fileFullNames = Directory.GetFiles(cleanupArg.DirPath, cleanupArg.SearchPattern, searchOption);

            Logger.LogInformation("total match file num: {FileNum}", fileFullNames.Length);

            foreach (var fileName in fileFullNames)
            {
                FileInfo fileInfo = new(fileName);
                if (fileInfo.LastWriteTime < DateTime.Now.AddHours(-cleanupArg.RetentionTime))
                {
                    Logger.LogInformation("file[{FileName}] won't be deleted because it does not meet the time requirement", fileName);
                    continue;
                }

                try
                {
                    File.Delete(fileName);
                    cleanCount++;
                    Logger.LogInformation("delete file[{FileName}] successfully!", fileName);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "delete file[{FileName}] failed!", fileName);
                }
            }
        }

        stopwatch.Stop();

        return Task.FromResult(ProcessorResult.OkMessage($"cost: {stopwatch.Elapsed}, clean: {cleanCount}"));
    }
}
