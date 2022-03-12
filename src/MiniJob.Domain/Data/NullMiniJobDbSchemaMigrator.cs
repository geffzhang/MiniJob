using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace MiniJob.Data
{
    /* This is used if database provider does't define
     * IMiniJobDbSchemaMigrator implementation.
     */
    public class NullMiniJobDbSchemaMigrator : IMiniJobDbSchemaMigrator, ITransientDependency
    {
        public Task MigrateAsync()
        {
            return Task.CompletedTask;
        }
    }
}