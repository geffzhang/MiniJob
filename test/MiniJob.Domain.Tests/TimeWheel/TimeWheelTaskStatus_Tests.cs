using Shouldly;
using Xunit;

namespace MiniJob.TimeWheel;

public class TimeWheelTaskStatus_Tests : MiniJobDomainTestBase
{
    /// <summary>
    /// 测试任务状态
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task TestTaskStatus()
    {
        var timer = GetRequiredService<ITimer>();

        var task1 = await timer.Schedule(TimeSpan.FromSeconds(5), async () =>
        {
            await Task.Delay(TimeSpan.FromSeconds(3));
        });
        var task2 = await timer.Schedule(TimeSpan.FromSeconds(5), () =>
        {
            throw new Exception();
        });
        var task3 = await timer.Schedule(TimeSpan.FromSeconds(5), () =>
        {
            throw new Exception();
        });

        await timer.Schedule(TimeSpan.FromSeconds(3), () => 
        {
            throw new Exception();
        });

        task1.TaskStatus.ShouldBe(TimeTaskStatus.Waiting);
        task2.TaskStatus.ShouldBe(TimeTaskStatus.Waiting);
        task3.TaskStatus.ShouldBe(TimeTaskStatus.Waiting);

        task3.Cancel();
        await Task.Delay(TimeSpan.FromSeconds(6));

        task1.TaskStatus.ShouldBe(TimeTaskStatus.Running);
        task2.TaskStatus.ShouldBe(TimeTaskStatus.Fail);
        task3.TaskStatus.ShouldBe(TimeTaskStatus.Cancel);

        await Task.Delay(TimeSpan.FromSeconds(4));
        task1.TaskStatus.ShouldBe(TimeTaskStatus.Success);

        timer.Stop();
    }
}
