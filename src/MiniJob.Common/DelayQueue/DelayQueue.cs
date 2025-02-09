﻿using System.Diagnostics;

namespace MiniJob.DelayQueue;

/// <summary>
/// 延时队列，线程安全，参考java DelayQueue实现
/// </summary>
/// <typeparam name="T"></typeparam>
public class DelayQueue<T> where T : class, IDelayItem
{
    private readonly object _lock = new object();

    /// <summary>
    /// 有序列表
    /// </summary>
    private readonly SortedQueue<T> _sortedList = new SortedQueue<T>();

    /// <summary>
    /// 当前排队等待取元素的线程
    /// </summary>
    private Thread _waitThread = null;

    /// <summary>
    /// 队列当前元素数量
    /// </summary>
    public int Count
    {
        get
        {
            lock (_lock)
            {
                return _sortedList.Count;
            }
        }
    }

    /// <summary>
    /// 队列是否为空
    /// </summary>
    public bool IsEmpty => Count == 0;

    /// <summary>
    /// 添加项
    /// </summary>
    /// <param name="item">要添加的项</param>
    /// <param name="cancelToken"></param>
    /// <returns>如果将 item 添加到集内，则为 true；否则为 false</returns>
    public bool TryAdd(T item, CancellationToken cancelToken = default)
    {
        return TryAdd(item, Timeout.InfiniteTimeSpan, cancelToken);
    }

    /// <summary>
    /// 添加项
    /// </summary>
    /// <param name="item">要添加的项</param>
    /// <param name="timeout">该方法执行超时时间</param>
    /// <param name="cancelToken"></param>
    /// <returns>如果将 item 添加到集内，则为 true；否则为 false</returns>
    /// <exception cref="ArgumentException"></exception>
    public bool TryAdd(T item, TimeSpan timeout, CancellationToken cancelToken = default)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        if (IsTimeout(timeout, cancelToken))
        {
            throw new ArgumentException("Method execute timeout or cancelled");
        }

        if (!Monitor.TryEnter(_lock, timeout))
        {
            return false;
        }

        if (cancelToken.IsCancellationRequested)
        {
            Monitor.Exit(_lock);
            return false;
        }

        try
        {
            if (_sortedList.TryAdd(item))
            {
                // 如果是首项，则唤醒就绪队列的首个线程准备获取锁
                if (Peek() == item)
                {
                    _waitThread = null;
                    Monitor.Pulse(_lock);
                }

                return true;
            }

            return false;
        }
        finally
        {
            Monitor.Exit(_lock);
        }
    }

    /// <summary>
    /// 取出首项，但不移除
    /// </summary>
    /// <returns>取出的项</returns>
    public T Peek()
    {
        lock (_lock)
        {
            return _sortedList.FirstOrDefault();
        }
    }

    /// <summary>
    /// 取出首项，但不移除
    /// </summary>
    /// <param name="item">取出的项</param>
    /// <returns>如果取到了项则为 true; 否则为 false</returns>
    public bool TryPeek(out T item)
    {
        item = Peek();
        return item != null;
    }

    /// <summary>
    /// 非阻塞获取项
    /// </summary>
    /// <param name="item">取出的项或者空值</param>
    /// <returns>如果队列为空或者首项未到期则为 false; 否则为 true 并且取出值</returns>
    public bool TryTakeNoBlocking(out T item)
    {
        lock (_lock)
        {
            item = Peek();
            if (item == null || item.GetDelaySpan() > TimeSpan.Zero)
            {
                item = null;
                return false;
            }
            return _sortedList.Remove(item);
        }
    }

    /// <summary>
    /// 取出项，如果未到期，则阻塞
    /// </summary>
    /// <param name="item">取出的项</param>
    /// <param name="cancelToken"></param>
    /// <returns></returns>
    public bool TryTake(out T item, CancellationToken cancelToken = default)
    {
        item = null;

        if (!Monitor.TryEnter(_lock))
        {
            return false;
        }

        if (cancelToken.IsCancellationRequested)
        {
            Monitor.Exit(_lock);
            return false;
        }

        try
        {
            while (!cancelToken.IsCancellationRequested)
            {
                // 当前没有项，阻塞等待
                if (!TryPeek(out item))
                {
                    Monitor.Wait(_lock);
                    continue;
                }

                // 如果已经到期，则出队
                var delaySpan = item.GetDelaySpan();
                if (delaySpan <= TimeSpan.Zero)
                {
                    return _sortedList.Remove(item);
                }

                // 移除引用，便于GC清理
                item = null;

                // 如果有其它线程也在等待，则阻塞等待
                if (_waitThread != null)
                {
                    Monitor.Wait(_lock);
                    continue;
                }

                // 否则当前线程设为等待线程
                var thisThread = Thread.CurrentThread;
                _waitThread = thisThread;

                try
                {
                    // 阻塞等待，如果有更早的项加入，会提前释放
                    // 否则等待delayMs时间，即当前项到期
                    // 注意，这里不能直接返回当前项，因为当前项可能被其它线程取出，所以要进入下一个循环获取
                    Monitor.Wait(_lock, delaySpan);
                    continue;
                }
                finally
                {
                    // 释放出来，让其它线程也可以获取
                    if (_waitThread == thisThread)
                    {
                        _waitThread = null;
                    }
                }
            }

            return false;
        }
        finally
        {
            // 当前线程已取到项，且还有剩余项，则唤醒其它就绪的线程
            if (_waitThread == null && _sortedList.Count > 0)
            {
                Monitor.Pulse(_lock);
            }

            Monitor.Exit(_lock);
        }
    }

    /// <summary>
    /// 取出项，如果未到期，则阻塞
    /// </summary>
    /// <param name="item"></param>
    /// <param name="timeout">该方法执行超时时间，注意，实际超时时间可能大于指定值</param>
    /// <param name="cancelToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public bool TryTake(out T item, TimeSpan timeout, CancellationToken cancelToken = default)
    {
        item = null;

        if (IsTimeout(timeout, cancelToken))
        {
            throw new ArgumentException("Method execute timeout or cancelled");
        }

        if (!Monitor.TryEnter(_lock, timeout))
        {
            return false;
        }

        if (IsTimeout(timeout, cancelToken))
        {
            Monitor.Exit(_lock);
            return false;
        }

        try
        {
            while (!IsTimeout(timeout, cancelToken))
            {
                // 当前没有项，阻塞等待
                if (!TryPeek(out item))
                {
                    timeout = Wait(_lock, timeout);
                    continue;
                }

                // 如果已经到期，则出队
                var delaySpan = item.GetDelaySpan();
                if (delaySpan <= TimeSpan.Zero)
                {
                    return _sortedList.Remove(item);
                }

                // 移除引用，便于GC清理
                item = null;

                // 如果有其它线程也在等待，则阻塞等待
                if (timeout < delaySpan || _waitThread != null)
                {
                    timeout = Wait(_lock, timeout);
                    continue;
                }

                // 否则当前线程设为等待线程
                var thisThread = Thread.CurrentThread;
                _waitThread = thisThread;

                try
                {
                    // 阻塞等待，如果有更早的项加入，会提前释放
                    // 否则等待delayMs时间，即当前项到期
                    // 注意，这里不能直接返回当前项，因为当前项可能被其它线程取出，所以要进入下一个循环获取
                    var timeLeft = Wait(_lock, delaySpan);
                    timeout -= delaySpan - timeLeft;
                    continue;
                }
                finally
                {
                    // 释放出来，让其它线程也可以获取
                    if (_waitThread == thisThread)
                    {
                        _waitThread = null;
                    }
                }
            }

            return false;
        }
        finally
        {
            // 当前线程已取到项，且还有剩余项，则唤醒其它就绪的线程
            if (_waitThread == null && _sortedList.Count > 0)
            {
                Monitor.Pulse(_lock);
            }

            Monitor.Exit(_lock);
        }
    }

    /// <summary>
    /// 清理
    /// </summary>
    public void Clear()
    {
        lock (_lock)
        {
            _sortedList.Clear();
        }
    }

    /// <summary>
    /// 是否超时
    /// </summary>
    /// <param name="timeout"></param>
    /// <param name="cancelToken"></param>
    /// <returns></returns>
    private static bool IsTimeout(TimeSpan timeout, CancellationToken cancelToken)
    {
        return (timeout <= TimeSpan.Zero && timeout != Timeout.InfiniteTimeSpan) ||
               cancelToken.IsCancellationRequested;
    }

    /// <summary>
    /// 锁等待，返回剩余时间
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="timeout">超时时间，如果是Infinite则无限期等待</param>
    /// <returns></returns>
    private static TimeSpan Wait(object obj, TimeSpan timeout)
    {
        // Monitor.Wait阻塞并释放锁，将当前线程置于等待队列，直至Monitor.Pulse通知其进入就绪队列，
        // 或者超时未接到通知，自动进入就绪队列。
        // timeout是进入就绪队列之前等待的时间，返回false表示已超时。
        // 进入就绪队列后会尝试获取锁，但直至拿到锁之前都不会返回值。

        var sw = Stopwatch.StartNew();
        Monitor.Wait(obj, timeout);
        sw.Stop();

        return timeout - sw.Elapsed;
    }
}
