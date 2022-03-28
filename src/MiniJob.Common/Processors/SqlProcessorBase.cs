using Microsoft.Extensions.Logging;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace MiniJob.Processors;

/// <summary>
/// Sql 处理器
/// 解析参数 => 校验参数 => 解析 SQL => 校验 SQL => 执行 SQL
/// <para>可以通过 <see cref="AddSqlValidator(string, Func{string, bool})"/> 方法添加SQL校验器拦截非法SQL</para>
/// <para>可以通过指定 <see cref="SqlParser"/> 来实现定制SQL解析逻辑的需求（如参数替换等）</para>
/// </summary>
public abstract class SqlProcessorBase : ProcessorBase
{
    /// <summary>
    /// 获取数据库连接
    /// 注意，此方法不会释放Connection对象
    /// </summary>
    /// <param name="sqlArgs"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    protected abstract IDbConnection CreateConnection(SqlArgs sqlArgs, ProcessorContext context);

    /// <summary>
    /// 自定义 SQL 解析器
    /// </summary>
    protected virtual ISqlParser SqlParser { get; set; }

    /// <summary>
    /// Sql 验证器
    /// </summary>
    protected virtual Dictionary<string, Func<string, bool>> SqlValidators { get; set; } = new Dictionary<string, Func<string, bool>>();

    protected override Task<ProcessorResult> DoWorkAsync(ProcessorContext context)
    {
        // 解析参数
        var sqlArgs = JsonSerializer.Deserialize<SqlArgs>(context.GetArgs());

        Logger.LogInformation("origin sql args: {@Args}", sqlArgs);

        // 校验参数
        ValidateArgs(sqlArgs);

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        // 解析
        if (SqlParser != null)
        {
            Logger.LogInformation("before parse sql: {Sql}", sqlArgs.Sql);
            var sqlResult = SqlParser.Parse(sqlArgs.Sql, context);
            sqlArgs.Sql = sqlResult;
            Logger.LogInformation("after parse sql: {Sql}", sqlResult);
        }

        // 校验Sql
        ValidateSql(sqlArgs.Sql);

        // 执行
        ExecuteSql(sqlArgs, context);

        stopwatch.Stop();
        var message = $"execute successfully, used time: {stopwatch.Elapsed}";
        Logger.LogInformation(message);

        return Task.FromResult(ProcessorResult.OkMessage(message));
    }

    /// <summary>
    /// 校验参数，如果校验不通过直接抛异常
    /// </summary>
    /// <param name="sqlArgs"></param>
    protected virtual void ValidateArgs(SqlArgs sqlArgs)
    {

    }

    /// <summary>
    /// 使用 Sql 验证器校验 SQL 合法性
    /// </summary>
    /// <param name="sql"></param>
    protected virtual void ValidateSql(string sql)
    {
        foreach (var sqlValidator in SqlValidators)
        {
            if (!sqlValidator.Value(sql))
            {
                Logger.LogError("validate sql by validator[{ValidatorName}] failed, skip to execute!", sqlValidator.Key);
                throw new MiniJobException($"illegal sql, can't pass the validation of {sqlValidator.Key}");
            }
        }
    }

    /// <summary>
    /// 添加一个 SQL 验证器
    /// </summary>
    /// <param name="validatorName">验证器名称</param>
    /// <param name="sqlValidator">验证器</param>
    public void AddSqlValidator(string validatorName, Func<string, bool> sqlValidator)
    {
        SqlValidators.Add(validatorName, sqlValidator);
        Logger.LogInformation("add sql validator({ValidatorName})' successfully.", validatorName);
    }

    /// <summary>
    /// 执行 SQL
    /// </summary>
    /// <param name="sqlArgs">SQL Processor 参数信息</param>
    /// <param name="context">任务上下文</param>
    protected virtual void ExecuteSql(SqlArgs sqlArgs, ProcessorContext context)
    {
        try
        {
            using var connection = CreateConnection(sqlArgs, context);
            if (connection.State != ConnectionState.Open)
                connection.Open();

            using var command = connection.CreateCommand();
            command.CommandTimeout = sqlArgs.Timeout;
            command.CommandText = sqlArgs.Sql;

            if (sqlArgs.IsQuery)
            {
                using var dataReader = command.ExecuteReader();
                OutputSqlResult(dataReader);
            }
            else
            {
                var affectedRow = command.ExecuteNonQuery();
                Logger.LogInformation("affected row count: {Count}", affectedRow);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "execute sql failed");
            throw;
        }
    }

    /// <summary>
    /// 输出SQL结果
    /// </summary>
    /// <param name="dataReader">Sql结果数据集</param>
    protected virtual void OutputSqlResult(IDataReader dataReader)
    {
        Logger.LogInformation("====== SQL EXECUTE RESULT ======");

        DataTable schemaTable = dataReader.GetSchemaTable();
        List<string> ColumnNames = new();

        StringBuilder stringBuilder = new();
        foreach (DataRow myField in schemaTable.Rows)
        {
            var columnName = myField["ColumnName"].ToString();
            ColumnNames.Add(columnName);
        }
        stringBuilder.AppendLine($"[Columns] {ColumnNames.JoinAsString(",")}");

        List<object> values = new();
        int row = 0;
        while (dataReader.Read())
        {
            row++;
            foreach (var column in ColumnNames)
            {
                values.Add(dataReader[column]);
            }
            stringBuilder.AppendLine($"[Row-{row}] {values.JoinAsString(",")}");
            values.Clear();
        }
        Logger.LogInformation(stringBuilder.ToString());

        Logger.LogInformation("====== SQL EXECUTE RESULT ======");
    }
}
