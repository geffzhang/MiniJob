using Microsoft.Extensions.Logging;

namespace MiniJob.Processors;

/// <summary>
/// 用于测试的任务处理器
/// </summary>
[JobConfig("Test", 
    Description = "测试任务",
    TimeExpressionType = Entities.TimeExpressionType.FixedRate,
    TimeExpressionValue = "120")]
public class TestProcessor : ProcessorBase
{
    protected override Task<ProcessorResult> DoWorkAsync(ProcessorContext context)
    {
        Logger.LogInformation("TestProcessor work");
        return Task.FromResult(ProcessorResult.OkMessage("TestProcessor should be work"));
    }
}
