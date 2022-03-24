namespace MiniJob.Cron.Parsers;

/// <summary>
/// Cron 字段值含 {0}L 字符解析器
/// </summary>
/// <remarks>
/// <para>表示月中最后一个星期{0}，仅在 <see cref="CrontabFieldKind.DayOfWeek"/> 字段域中使用</para>
/// </remarks>
internal sealed class LastDayOfWeekInMonthParser : CronParserBase
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="dayOfWeek">星期，0 = 星期天，7 = 星期六</param>
    /// <param name="kind">Cron 字段种类</param>
    /// <exception cref="CronExpressionException"></exception>
    public LastDayOfWeekInMonthParser(int dayOfWeek, CrontabFieldKind kind)
    {
        // 验证 {0}L 字符是否在 DayOfWeek 字段域中使用
        if (kind != CrontabFieldKind.DayOfWeek)
        {
            throw new CronExpressionException(string.Format("The <{0}L> parser can only be used in the Day of Week field.", dayOfWeek));
        }

        DayOfWeek = dayOfWeek;
        DateTimeDayOfWeek = ToDayOfWeek(dayOfWeek);
        Kind = kind;
    }

    /// <summary>
    /// 星期
    /// </summary>
    public int DayOfWeek { get; }

    /// <summary>
    /// <see cref="DayOfWeek"/> 类型星期
    /// </summary>
    private DayOfWeek DateTimeDayOfWeek { get; }

    /// <summary>
    /// 判断当前时间是否符合 Cron 字段种类解析规则
    /// </summary>
    /// <param name="datetime">当前时间</param>
    /// <returns><see cref="bool"/></returns>
    public override bool IsMatch(DateTime datetime)
    {
        return datetime.Day == LastDayOfMonth(DateTimeDayOfWeek, datetime.Year, datetime.Month);
    }

    /// <summary>
    /// 将解析器转换成字符串输出
    /// </summary>
    /// <returns><see cref="string"/></returns>
    public override string ToString()
    {
        return string.Format("{0}L", DayOfWeek);
    }
}
