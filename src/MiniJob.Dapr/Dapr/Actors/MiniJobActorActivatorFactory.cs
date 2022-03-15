using Dapr.Actors.Runtime;

namespace MiniJob.Dapr.Actors;

internal class MiniJobActorActivatorFactory : ActorActivatorFactory
{
    private readonly IServiceProvider services;

    public MiniJobActorActivatorFactory(IServiceProvider services)
    {
        this.services = services;
    }

    public override ActorActivator CreateActivator(ActorTypeInformation type)
    {
        return new MiniJobActorActivator(services, type);
    }
}