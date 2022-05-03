using Dapr.Actors;
using Dapr.Actors.Runtime;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Volo.Abp.DependencyInjection;

namespace MiniJob.Dapr.Actors;

/// <summary>
/// 自动注册继承自 <see cref="IActor"/> 的Actor(调用<see cref="ActorRegistrationCollection.RegisterActor{TActor}(Action{ActorRegistration})"/>)
/// <para>添加 <see cref="DisableAutoRegisterActorAttribute"/> 特性禁用自动注册</para>
/// </summary>
public class ActorConventionalRegistrar : DefaultConventionalRegistrar
{
    private static readonly MethodInfo daprRegisterMethodInfo = 
        typeof(ActorRegistrationCollection).GetMethod("RegisterActor", new Type[] { typeof(Action<ActorRegistration>) });

    public override void AddType(IServiceCollection services, Type type)
    {
        if (IsConventionalRegistrationDisabled(type))
        {
            return;
        }

        services.AddActors(options => CreateDelegate(options.Actors, type));
    }

    protected override bool IsConventionalRegistrationDisabled(Type type)
    {
        return !IsActor(type) ||
            IsDisabledRegisterActor(type) ||
            base.IsConventionalRegistrationDisabled(type);
    }

    private static bool IsActor(Type type)
    {
        return typeof(IActor).IsAssignableFrom(type);
    }

    private static bool IsDisabledRegisterActor(Type type)
    {
        return type.IsDefined(typeof(DisableAutoRegisterActorAttribute), true);
    }

    internal static void CreateDelegate(ActorRegistrationCollection actorRegistrations, Type actorType)
    {
        daprRegisterMethodInfo.MakeGenericMethod(actorType).Invoke(actorRegistrations, new object[] { default(Action<ActorRegistration>) });
    }
}