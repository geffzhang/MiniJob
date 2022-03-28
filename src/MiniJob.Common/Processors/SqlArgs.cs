namespace MiniJob.Processors;

public class SqlArgs
{
    /// <summary>
    /// 数据源名称，默认为 Default
    /// </summary>
    public virtual string DataSourceName { get; set; }

    /// <summary>
    /// 数据提供者，不能为空 支持SqlServer,MySql,Oracle,PostgreSql,Sqlite
    /// </summary>
    public virtual string DatabaseProvider { get; set; }

    /// <summary>
    /// 连接字符串
    /// </summary>
    public virtual string ConnectionString { get; set; }

    /// <summary>
    /// 需要执行的 SQL
    /// </summary>
    public virtual string Sql { get; set; }

    /// <summary>
    /// 超时时间，默认为30秒
    /// </summary>
    public virtual int Timeout { get; set; }

    /// <summary>
    /// 是否数据查询，默认为否
    /// 分为数据查询和数据更新两类，数据查询将展示执行结果，请勿查询大量数据
    /// </summary>
    public virtual bool IsQuery { get; set; }

    public SqlArgs()
    {
        Timeout = 30;
        IsQuery = false;
        DataSourceName = "Default";
    }
}
