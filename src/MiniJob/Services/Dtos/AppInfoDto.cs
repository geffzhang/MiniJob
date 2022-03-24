using Volo.Abp.Application.Dtos;

namespace MiniJob.Jobs;

public class AppInfoDto : AuditedEntityDto<Guid>
{
    /// <summary>
    /// 应用名称
    /// </summary>
    public virtual string AppName { get; set; }

    /// <summary>
    /// 应用描述
    /// </summary>
    public virtual string Description { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public virtual bool IsEnabled { get; set; }
}