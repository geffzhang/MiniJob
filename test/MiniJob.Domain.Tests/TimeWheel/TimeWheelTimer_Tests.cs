using Shouldly;
using Xunit;

namespace MiniJob.TimeWheel;

public class TimeWheelTimer_Tests : MiniJobDomainTestBase
{
    /// <summary>
    /// 测试时间轮
    /// </summary>
    [Fact]
    public async Task AddTaskTest()
    {
        var outputs = new Dictionary<string, DateTime>();

        var timer = GetRequiredService<ITimer>();

        outputs.Add("00", DateTime.Now);

        await timer.Schedule(TimeSpan.FromMilliseconds(5000), () =>
        {
            outputs.Add("20", DateTime.Now);
            return Task.CompletedTask;
        });
        await timer.Schedule(TimeSpan.FromMilliseconds(2000), () =>
        {
            outputs.Add("11", DateTime.Now);
            return Task.CompletedTask;
        });

        await timer.Schedule(TimeSpan.FromSeconds(12), () =>
        {
            outputs.Add("30", DateTime.Now);
            return Task.CompletedTask;
        });
        await timer.Schedule(TimeSpan.FromSeconds(2), () =>
        {
            outputs.Add("12", DateTime.Now);
            return Task.CompletedTask;
        });

        await Task.Delay(TimeSpan.FromSeconds(15));
        timer.Stop();

        outputs.Add("99", DateTime.Now);

        Console.WriteLine(string.Join(Environment.NewLine, outputs.Select(o => $"{o.Key}, {o.Value:HH:mm:ss.ffff}")));

        outputs.Count.ShouldBe(6);
        Calc(outputs.Skip(1).First().Value, outputs.First().Value).ShouldBe(2);
        Calc(outputs.Skip(2).First().Value, outputs.First().Value).ShouldBe(2);
        Calc(outputs.Skip(3).First().Value, outputs.First().Value).ShouldBe(5);
        Calc(outputs.Skip(4).First().Value, outputs.First().Value).ShouldBe(12);
    }

    private static int Calc(DateTime dt1, DateTime dt2)
    {
        return (int)(CutOffMillisecond(dt1) - CutOffMillisecond(dt2)).TotalSeconds;
    }

    /// <summary>
    /// 截掉毫秒部分
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    private static DateTime CutOffMillisecond(DateTime dt)
    {
        return new DateTime(dt.Ticks - (dt.Ticks % TimeSpan.TicksPerSecond), dt.Kind);
    }
}
