﻿namespace MiniJob.DelayQueue;

/// <summary>
/// 延时队列任务项
/// </summary>
public interface IDelayItem : IComparable
{
    /// <summary>
    /// 获取剩余延时
    /// </summary>
    /// <returns></returns>
    TimeSpan GetDelaySpan();
}
