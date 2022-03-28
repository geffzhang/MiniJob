using Microsoft.EntityFrameworkCore;
using MiniJob.Entities.Jobs;
using MiniJob.Services.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.ObjectMapping;

namespace MiniJob.Services.DomainServices;

public class ProcessorService : DomainService
{
    protected IRepository<ProcessorInfo, Guid> ProcessorRepository { get; }

    public ProcessorService(
        IRepository<ProcessorInfo, Guid> processorRepository)
    {
        ProcessorRepository = processorRepository;
    }

    public async Task RegisterProcessor(IEnumerable<ProcessorDto> processorDtos)
    {
        var appId = processorDtos.FirstOrDefault()?.AppId;
        var processors = await (await ProcessorRepository.GetQueryableAsync())
            .Where(p => p.AppId == appId)
            .ToListAsync();

        var objectMapper = LazyServiceProvider.LazyGetRequiredService<IObjectMapper>();

        foreach (var dto in processorDtos)
        {
            if (!processors.Any(p => p.AssemblyName == dto.AssemblyName && p.FullName == dto.FullName))
            {
                var processor = objectMapper.Map<ProcessorDto, ProcessorInfo>(dto);
                EntityHelper.TrySetId(
                    processor,
                    () => GuidGenerator.Create(),
                    true
                );
                await ProcessorRepository.InsertAsync(processor);
            }
        }
    }
}
