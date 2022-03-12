using MiniJob.Jobs;
using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace MiniJob.Data
{
    public class MiniJobDataSeederContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IRepository<AppInfo, Guid> _appRepository;

        public MiniJobDataSeederContributor(IRepository<AppInfo, Guid> appRepository)
        {
            _appRepository = appRepository;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (await _appRepository.GetCountAsync() <= 0)
            {
                await _appRepository.InsertAsync(
                    new AppInfo
                    {
                        AppName = "TestApp",
                        Description = "测试应用",
                        IsEnabled = true,
                    },
                    autoSave: true
                );
            }
        }
    }
}
