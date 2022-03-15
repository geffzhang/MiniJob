namespace MiniJob.Data;

public interface IMiniJobDbSchemaMigrator
{
    Task MigrateAsync();
}