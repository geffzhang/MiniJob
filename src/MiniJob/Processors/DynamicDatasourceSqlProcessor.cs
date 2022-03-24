using Microsoft.Data.Sqlite;
using MySqlConnector;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace MiniJob.Processors;

/// <summary>
/// 动态数据源SQL处理器
/// </summary>
public class DynamicDatasourceSqlProcessor : SqlProcessorBase
{
    private static readonly ConcurrentDictionary<string, DbProviderFactory> factoryCache = new();

    private static DbProviderFactory GetFactory(string providerName)
    {
        return providerName switch
        {
            MiniJobConsts.DatabaseSupport.SqlServer => SqlClientFactory.Instance,
            MiniJobConsts.DatabaseSupport.MySql => MySqlConnectorFactory.Instance,
            MiniJobConsts.DatabaseSupport.Oracle => OracleClientFactory.Instance,
            MiniJobConsts.DatabaseSupport.PostgreSql => NpgsqlFactory.Instance,
            MiniJobConsts.DatabaseSupport.Sqlite => SqliteFactory.Instance,
            _ => throw new NotSupportedException($"not support database provider: {providerName}"),
        };
    }

    protected override IDbConnection CreateConnection(SqlArgs sqlArgs, ProcessorContext context)
    {
        var connectionString = GetConnectionString(sqlArgs, context);

        var factory = factoryCache.GetOrAdd(sqlArgs.DatabaseProvider, GetFactory);
        var connection = factory.CreateConnection();

        if (connection == null)
        {
            throw new InvalidOperationException($"Database provider factory: '{sqlArgs.DatabaseProvider}' did not return a connection object.");
        }

        connection.ConnectionString = connectionString;
        return connection;
    }

    /// <summary>
    /// 获取连接字符串
    /// 优先使用ConnectionString，如果为空则根据配置的数据源名称从配置文件读取数据库连接字符串
    /// </summary>
    /// <param name="sqlArgs"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    /// <exception cref="MiniJobException"></exception>
    protected virtual string GetConnectionString(SqlArgs sqlArgs, ProcessorContext context)
    {
        if (sqlArgs.DatabaseProvider.IsNullOrWhiteSpace())
            throw new MiniJobException("DatabaseProvider can't be empty, please check args configuration.");

        if (sqlArgs.ConnectionString.IsNullOrWhiteSpace() && sqlArgs.DataSourceName.IsNullOrWhiteSpace())
            throw new MiniJobException("ConnectionString and DataSourceName cannot be empty at the same time.");

        if (sqlArgs.ConnectionString.IsNullOrWhiteSpace())
        {
            var configuration = LazyServiceProvider.LazyGetRequiredService<IConfiguration>();
            sqlArgs.ConnectionString = configuration.GetConnectionString(sqlArgs.DataSourceName);
            Logger.LogInformation("use datasource name: {Name}", sqlArgs.DataSourceName);
        }

        if (sqlArgs.ConnectionString.IsNullOrWhiteSpace())
            throw new MiniJobException("ConnectionString can't be empty, please check args configuration.");

        return sqlArgs.ConnectionString;
    }
}
