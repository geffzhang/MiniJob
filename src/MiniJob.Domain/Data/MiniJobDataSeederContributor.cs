using MiniJob.Enums;
using MiniJob.Jobs;
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

    public MiniJobDataSeederContributor(
        IRepository<AppInfo, Guid> appRepository,
        IGuidGenerator guidGenerator,
        IClock clock,
        IJsonSerializer jsonSerializer)
    {
        _appRepository = appRepository;
        GuidGenerator = guidGenerator;
        Clock = clock;
        JsonSerializer = jsonSerializer;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        if (await _appRepository.GetCountAsync() <= 0)
        {
            var appInfo = new AppInfo(GuidGenerator.Create(), "TestApp", "演示任务", context.TenantId);

            // Http 任务
            var httpArgs = new HttpArgs()
            {
                Url = "https://v1.hitokoto.cn/?c=k",
                CheckKey = "type",
                CheckValue = "k",
            };
            var httpJobInfo = new JobInfo(GuidGenerator.Create(), appInfo.Id, "Http任务示例", string.Empty, context.TenantId)
            {
                ProcessorType = ProcessorType.Http,
                JobArgs = JsonSerializer.Serialize(httpArgs),
                TimeExpression = TimeExpressionType.FixedRate,
                TimeExpressionValue = "120",
                Timeout = TimeSpan.FromSeconds(30),
                Description = "每两分钟获取一次一言接口数据",
                ProcessorInfo = typeof(HttpProcessor).FullName,
                MisfireStrategy = MisfireStrategy.FireOnceNow
            };
            httpJobInfo.CalculateNextTriggerTime(Clock.Now);
            appInfo.JobInfos.Add(httpJobInfo);

            // Shell 任务
            var shellJobInfo = new JobInfo(GuidGenerator.Create(), appInfo.Id, "Shell任务示例", string.Empty, context.TenantId)
            {
                ProcessorType = ProcessorType.Shell,
                JobArgs = "@echo off & echo Hello Minijob",
                TimeExpression = TimeExpressionType.FixedRate,
                TimeExpressionValue = "120",
                Timeout = TimeSpan.FromSeconds(30),
                Description = "每两分钟打印Hello MiniJob",
                ProcessorInfo = typeof(ShellProcessor).FullName,
                MisfireStrategy = MisfireStrategy.FireOnceNow
            };
            shellJobInfo.CalculateNextTriggerTime(Clock.Now);
            appInfo.JobInfos.Add(shellJobInfo);

            await _appRepository.InsertAsync(appInfo, true);
        }
    }
}