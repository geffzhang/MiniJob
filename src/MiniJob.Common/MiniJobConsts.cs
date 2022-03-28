namespace MiniJob;

/// <summary>
/// MiniJob 静态变量
/// </summary>
public static class MiniJobConsts
{
    public const bool IsMultiTenant = true;

    public const string DbTablePrefix = "MiniJob";

    public const string DbSchema = null;

    public static TimeSpan WorkerTimeout = TimeSpan.FromSeconds(30);

    /// <summary>
    /// 脚本路径
    /// </summary>
    public static readonly string ScriptWorkerDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ScriptProcessor");

    /// <summary>
    /// 下载协议
    /// </summary>
    public static readonly List<string> DownloadProtocol = new() { "http", "https", "ftp" };

    /// <summary>
    /// TaskTracker 长时间未上报
    /// </summary>
    public const string ReportTimeout = "worker report timeout, maybe TaskTracker down";

    /// <summary>
    /// 支持的数据库类型
    /// </summary>
    public static class DatabaseSupport
    {
        public const string SqlServer = "SqlServer";
        public const string MySql = "MySql";
        public const string Oracle = "Oracle";
        public const string PostgreSql = "PostgreSql";
        public const string Sqlite = "Sqlite";
    }
}