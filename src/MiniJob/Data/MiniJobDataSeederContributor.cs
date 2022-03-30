using Microsoft.Extensions.Options;
using MiniJob.Entities.Jobs;
using MiniJob.Processors;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Json;
using Volo.Abp.Timing;

namespace MiniJob.Data;

public class MiniJobDataSeederContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IRepository<AppInfo, Guid> _appRepository;
    protected IGuidGenerator GuidGenerator { get; }
    protected IClock Clock { get; }
    protected IJsonSerializer JsonSerializer { get; }
    protected MiniJobProcessorOptions ProcessorOptions { get; }

    public MiniJobDataSeederContributor(
        IRepository<AppInfo, Guid> appRepository,
        IGuidGenerator guidGenerator,
        IClock clock,
        IJsonSerializer jsonSerializer,
        IOptions<MiniJobProcessorOptions> options)
    {
        _appRepository = appRepository;
        GuidGenerator = guidGenerator;
        Clock = clock;
        JsonSerializer = jsonSerializer;
        ProcessorOptions = options.Value;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        await CreateDefaultAppAsync(context);
    }

    private async Task CreateDefaultAppAsync(DataSeedContext context)
    {
        if (await _appRepository.GetCountAsync() <= 0)
        {
            var appInfo = new AppInfo(GuidGenerator.Create(), "Default", "默认应用(不可删除)", context.TenantId);

            foreach (var processorConfiguration in ProcessorOptions.GetProcessors())
            {
                var processor = new ProcessorInfo(GuidGenerator.Create(), appInfo.Id, processorConfiguration.ProcessorType);
                appInfo.ProcessorInfos.Add(processor);
            }

            foreach (var job in ProcessorOptions.Jobs)
            {
                var jobInfo = new JobInfo(GuidGenerator.Create(), appInfo.Id, job.Name, context.TenantId)
                {
                    ProcessorType = Entities.ProcessorType.CSharp,
                    JobArgs = job.JobArgs,
                    TimeExpression = job.TimeExpressionType,
                    TimeExpressionValue = job.TimeExpressionValue,
                    MisfireStrategy = job.MisfireStrategy,
                    ProcessorInfo = job.ProcessorType.FullName,
                    Description = job.Description,
                    ExecuteType = job.ExecuteType,
                    JobPriority = job.JobPriority,
                    IsEnabled = job.IsEnabled,
                };
                jobInfo.CalculateNextTriggerTime(Clock.Now);
                appInfo.JobInfos.Add(jobInfo);
            }

            await _appRepository.InsertAsync(appInfo, true);
        }
    }
}