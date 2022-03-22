using Shouldly;
using System.Collections.Concurrent;
using Xunit;

namespace MiniJob.DelayQueue;

public class DelayQueue_Tests
{
    [Fact]
    public void TestDelayQueue()
    {
        var delayQueue = new DelayQueue<DelayItem<Action>>();

        // 输出列表
        var outputs = new Dictionary<string, DateTime>();
        outputs.Add("00", DateTime.Now);

        // 添加任务
        var item1 = new DelayItem<Action>(TimeSpan.FromSeconds(5), () => { outputs.Add("50", DateTime.Now); });
        var item2 = new DelayItem<Action>(TimeSpan.FromSeconds(2), () => { outputs.Add("20", DateTime.Now); });
        delayQueue.TryAdd(item1);
        delayQueue.TryAdd(item2);
        delayQueue.TryAdd(item2);

        delayQueue.TryAdd(new DelayItem<Action>(TimeSpan.FromSeconds(12), () => { outputs.Add("120", DateTime.Now); }));
        delayQueue.TryAdd(new DelayItem<Action>(TimeSpan.FromSeconds(2), () => { outputs.Add("21", DateTime.Now); }));

        delayQueue.Count.ShouldBe(4);

        // 获取任务
        while (delayQueue.Count > 0)
        {
            if (delayQueue.TryTake(out var task))
            {
                task.Item.Invoke();
            }
        }

        Console.WriteLine(string.Join(Environment.NewLine, outputs.Select(o => $"{o.Key}, {o.Value:HH:mm:ss.ffff}")));

        Calc(outputs.Skip(1).First().Value, outputs.First().Value).ShouldBe(2);
        Calc(outputs.Skip(2).First().Value, outputs.First().Value).ShouldBe(2);
        Calc(outputs.Skip(3).First().Value, outputs.First().Value).ShouldBe(5);
        Calc(outputs.Skip(4).First().Value, outputs.First().Value).ShouldBe(12);
    }

    /// <summary>
    /// 多线程测试
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task TestMultithreading()
    {
        var delayQueue = new DelayQueue<DelayItem<int>>();

        // 添加任务
        var taskCount = 20;
        for (int i = 0; i < taskCount; i++)
        {
            delayQueue.TryAdd(new DelayItem<int>(TimeSpan.FromSeconds(i + 2), i));
        }

        delayQueue.Count.ShouldBe(taskCount);

        // 10个线程来消费
        var outputs = new ConcurrentDictionary<int, int>();
        var tasks = new List<Task>();
        for (int i = 0; i < 10; i++)
        {
            tasks.Add(Task.Factory.StartNew(() =>
            {
                while (delayQueue.Count > 0)
                {
                    if (delayQueue.TryTake(out var task, TimeSpan.FromSeconds(5)))
                    {
                        outputs.TryAdd(task.Item, Thread.CurrentThread.ManagedThreadId);
                    }
                }
            }, TaskCreationOptions.LongRunning));
        }

        await Task.WhenAll(tasks);

        delayQueue.Count.ShouldBe(0);
        outputs.Count.ShouldBe(taskCount);

        var preKey = -1;
        foreach (var output in outputs)
        {
            output.Key.ShouldBeGreaterThan(preKey);
            preKey = output.Key;
        }

        // 打印每个线程消费的任务数量
        Console.WriteLine(string.Join(Environment.NewLine,
            outputs.GroupBy(o => o.Value).Select(g => $"{g.Key}, {g.Count()}")));
    }

    private static int Calc(DateTime dt1, DateTime dt2)
    {
        // 毫秒数是存在误差的，这里统计秒数
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
