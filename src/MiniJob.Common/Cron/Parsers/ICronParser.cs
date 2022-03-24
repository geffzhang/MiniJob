namespace MiniJob.Cron.Parsers;

/// <summary>
/// Cron 字段字符解析器依赖接口
/// </summary>
internal interface ICronParser
{
    /// <summary>
    /// Cron 字段种类
    /// </summary>
    CrontabFieldKind Kind { get; }

    /// <summary>
    /// 判断当前时间是否符合 Cron 字段种类解析规则
    /// </summary>
    /// <param name="datetime">当前时间</param>
    /// <returns><see cref="bool"/></returns>
    bool IsMatch(DateTime datetime);
}
