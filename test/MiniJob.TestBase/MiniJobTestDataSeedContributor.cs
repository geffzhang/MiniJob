using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;

namespace MiniJob;

public class MiniJobTestDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    public Task SeedAsync(DataSeedContext context)
    {
        /* Seed additional test data... */

        return Task.CompletedTask;
    }
}