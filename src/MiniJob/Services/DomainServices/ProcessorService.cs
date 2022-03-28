using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MiniJob.Entities.Jobs;
using MiniJob.Processors;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace MiniJob.Services.DomainServices;

public class ProcessorService : DomainService
{
    protected MiniJobProcessorOptions ProcessorOptions { get; }

    protected IRepository<ProcessorInfo, Guid> ProcessorRepository { get; }

    public ProcessorService(
        IOptions<MiniJobProcessorOptions> options,
        IRepository<ProcessorInfo, Guid> processorRepository)
    {
        ProcessorOptions = options.Value;
        ProcessorRepository = processorRepository;
    }

    public async Task RegisterProcessor()
    {
        var processors = await (await ProcessorRepository.GetQueryableAsync()).ToListAsync();

        foreach (var processorConfiguration in ProcessorOptions.GetProcessors())
        {
            if (!processors.Any(p => p.WorkerName == processorConfiguration.ProcessorType.Assembly.FullName &&
                p.FullName == processorConfiguration.ProcessorType.FullName))
            {
                var processor = new ProcessorInfo(GuidGenerator.Create(), processorConfiguration.ProcessorType);
                await ProcessorRepository.InsertAsync(processor);
            }
        }
    }
}
