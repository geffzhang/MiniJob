namespace MiniJob.Processors;

/// <summary>
/// SQL 解析器
/// </summary>
public interface ISqlParser
{
    /// <summary>
    /// 自定义 SQL 解析逻辑
    /// </summary>
    /// <param name="sql">原始 SQL 语句</param>
    /// <param name="context">任务上下文</param>
    /// <returns>解析后的 SQL</returns>
    string Parse(string sql, ProcessorContext context);
}
