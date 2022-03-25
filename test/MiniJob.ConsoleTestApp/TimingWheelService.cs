using MiniJob.TimeWheel;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace MiniJob.HttpApi.Client.ConsoleTestApp;

public class TimingWheelService : ITransientDependency
{
    private readonly ITimer _timer;

    public TimingWheelService(ITimer timer)
    {
        _timer = timer;
    }

    public async Task RunAsync()
    {
        // 多线程测试
        var tasks = new List<Task>();
        for (int i = 0; i < 20; i++)
        {
            tasks.Add(Task.Factory.StartNew(() => AddTasks(_timer).GetAwaiter().GetResult(),
                TaskCreationOptions.LongRunning));
        }

        await Task.WhenAll(tasks);
        Console.WriteLine("任务添加完毕！");

        while (_timer.TaskCount > 0)
        {
            Console.WriteLine($"[{DateTime.Now}] 剩余任务数：{_timer.TaskCount} {GC.GetTotalMemory(false) / 1024 / 1024}MB");

            await Task.Delay(TimeSpan.FromSeconds(10));
        }

        _timer.Stop();
    }

    private static async Task AddTasks(ITimer timer)
    {
        var taskCount = 0;
        var threadId = Thread.CurrentThread.ManagedThreadId;

        for (int i = 0; i < 6000; i++, taskCount++)
        {
            var now = DateTime.Now;
            var delay = TimeSpan.FromSeconds(Random.Shared.Next(1, 60));

            await timer.Schedule(delay, () =>
            {
                var runTime = DateTime.Now;
                var actualDelay = CutOffMillisecond(runTime) - CutOffMillisecond(now);
                var actualDelay2 = runTime - now;

                //Console.WriteLine($"添加任务线程：{threadId}，" +
                //                  $"起始时间：{now:HH:mm:ss.fff}，" +
                //                  $"执行时间：{runTime:HH:mm:ss.fff}，" +
                //                  $"预期延时：{delay.TotalSeconds}s，" +
                //                  $"实际延时：{actualDelay.TotalSeconds}s，" +
                //                  $"精确延时：{actualDelay2.TotalSeconds}s");
            });
        }

        Console.WriteLine($"[{DateTime.Now}][线程{threadId}] 累计任务数：{taskCount}");
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
