using Microsoft.EntityFrameworkCore;
using Volo.Abp.DependencyInjection;

namespace MiniJob.Data;

public class MiniJobEFCoreDbSchemaMigrator : ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public MiniJobEFCoreDbSchemaMigrator(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolving the MiniJobDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<MiniJobDbContext>()
            .Database
            .MigrateAsync();
    }
}
