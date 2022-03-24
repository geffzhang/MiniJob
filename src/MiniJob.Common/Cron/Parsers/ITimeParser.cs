namespace MiniJob.Cron.Parsers;

/// <summary>
/// DateTime 时间解析器依赖接口
/// </summary>
/// <remarks>主要用于计算 DateTime 主要组成部分（秒，分，时）的下一个取值</remarks>
internal interface ITimeParser
{
    /// <summary>
    /// 获取 Cron 字段种类当前值的下一个发生值
    /// </summary>
    /// <param name="currentValue">时间值</param>
    /// <returns><see cref="int"/></returns>
    /// <exception cref="CronExpressionException"></exception>
    int? Next(int currentValue);

    /// <summary>
    /// 获取 Cron 字段种类字段起始值
    /// </summary>
    /// <returns><see cref="int"/></returns>
    /// <exception cref="CronExpressionException"></exception>
    int First();
}
