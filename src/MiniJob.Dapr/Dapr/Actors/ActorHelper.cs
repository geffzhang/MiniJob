using Dapr.Actors;
using Dapr.Actors.Client;
using System.Reflection;

namespace MiniJob.Dapr.Actors;

public class ActorHelper
{
    protected static MethodInfo CallCreateDefaultActorMethod { get; }

    static ActorHelper()
    {
        CallCreateDefaultActorMethod = typeof(ActorHelper)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
            .First(m => m.Name == nameof(CreateDefaultActor) && m.IsGenericMethodDefinition);
    }

    public static TActorInterface CreateActor<TActorInterface, TImpl>(string actorId)
        where TActorInterface : IActor
    {
        return CreateActor<TActorInterface>(actorId, typeof(TImpl));
    }

    public static TActorInterface CreateActor<TActorInterface>(string actorId, Type implType)
        where TActorInterface : IActor
    {
        return CreateActor<TActorInterface>(actorId, implType.Name);
    }

    public static TActorInterface CreateActor<TActorInterface>(string actorId, string actorType)
        where TActorInterface : IActor
    {
        return CreateActor<TActorInterface>(new ActorId(actorId), actorType);
    }

    public static TActorInterface CreateActor<TActorInterface>(ActorId actorId, string actorType)
        where TActorInterface : IActor
    {
        return ActorProxy.Create<TActorInterface>(actorId, actorType);
    }

    /// <summary>
    /// 使用 <see cref="Guid.Empty"/> 作为ActorId创建默认Actor
    /// </summary>
    /// <remarks>单例Actor的时候可使用此方法</remarks>
    /// <typeparam name="TActorInterface">接口类型</typeparam>
    /// <typeparam name="TImpl">实现类型</typeparam>
    /// <returns></returns>
    public static TActorInterface CreateDefaultActor<TActorInterface, TImpl>()
        where TActorInterface : IActor
    {
        return CreateActor<TActorInterface>(Guid.Empty.ToString(), typeof(TImpl));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="implType"></param>
    /// <returns></returns>
    public static object CreateDefaultActor(Type implType)
    {
        if (implType.IsAssignableFrom(typeof(IActor)))
            throw new Exception($"actor type {implType} must implementation IActor interface.");

        var actorInterfaceType = implType.GetInterface("I" + implType.Name);
        if (actorInterfaceType is null)
        {
            foreach (var type in implType.GetInterfaces())
            {
                if (type.GetInterfaces().Contains(typeof(IActor)))
                {
                    actorInterfaceType = type;
                    break;
                }
            }
        }

        return CallCreateDefaultActorMethod.MakeGenericMethod(actorInterfaceType, implType)
            .Invoke(null, null);
    }
}