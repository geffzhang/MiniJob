using System.Threading.Tasks;

namespace MiniJob.Data
{
    public interface IMiniJobDbSchemaMigrator
    {
        Task MigrateAsync();
    }
}