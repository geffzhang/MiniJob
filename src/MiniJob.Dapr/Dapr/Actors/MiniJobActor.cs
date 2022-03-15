using Dapr.Actors.Runtime;
using Volo.Abp.Auditing;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Timing;
using Volo.Abp.Uow;
using Volo.Abp.Users;

namespace MiniJob.Dapr.Actors;

/// <summary>
/// Actor 基类，添加工作单元、审计日志支持，并添加了一些常用属性
/// <para>所有 Actor 应继承此类</para>
/// </summary>
public abstract class MiniJobActor :
    Actor,
    IUnitOfWorkEnabled,
    IAuditingEnabled,
    ITransientDependency
{
    public IAbpLazyServiceProvider LazyServiceProvider { get; set; }

    protected IUnitOfWorkManager UnitOfWorkManager => LazyServiceProvider.LazyGetRequiredService<IUnitOfWorkManager>();

    protected IGuidGenerator GuidGenerator => LazyServiceProvider.LazyGetService<IGuidGenerator>(SimpleGuidGenerator.Instance);

    protected ICurrentTenant CurrentTenant => LazyServiceProvider.LazyGetRequiredService<ICurrentTenant>();

    protected ICurrentUser CurrentUser => LazyServiceProvider.LazyGetRequiredService<ICurrentUser>();

    protected IClock Clock => LazyServiceProvider.LazyGetRequiredService<IClock>();

    protected IUnitOfWork CurrentUnitOfWork => UnitOfWorkManager?.Current;

    public MiniJobActor(ActorHost host)
        : base(host)
    {

    }
}