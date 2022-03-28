using Volo.Abp.Application.Dtos;

namespace MiniJob.Services.Dtos;

public class ProcessorDto : AuditedEntityDto<Guid>
{
    public virtual Guid AppId { get; set; }

    public virtual string Name { get; set; }

    public virtual string FullName { get; set; }

    public virtual string AssemblyName { get; set; }

    public virtual bool IsEnabled { get; set; }
}
