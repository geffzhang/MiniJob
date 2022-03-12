using Autofac;
using Autofac.Extensions.DependencyInjection;
using Dapr.Actors.Runtime;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MiniJob.Dapr.Actors
{
    /// <summary>
    /// 自定义Actor激活器，以便使用代理和拦截器
    /// </summary>
    internal class MiniJobActorActivator : ActorActivator
    {
        private readonly IServiceProvider services;
        private readonly ActorTypeInformation type;
        private readonly Func<ObjectFactory> initializer;

        // factory is used to create the actor instance - initialization of the factory is protected
        // by the initialized and @lock fields.
        //
        // This serves as a cache for the generated code that constructs the object from DI.
        private ObjectFactory factory;
        private bool initialized;
        private object @lock;

        public MiniJobActorActivator(IServiceProvider services, ActorTypeInformation type)
        {
            this.services = services;
            this.type = type;

            // Will be invoked to initialize the factory.
            initializer = () =>
            {
                return ActivatorUtilities.CreateFactory(this.type.ImplementationType, new Type[] { typeof(ActorHost), });
            };
        }

        public override async Task<ActorActivatorState> CreateAsync(ActorHost host)
        {
            if (services is AutofacServiceProvider autofacService)
            {
                var scope = autofacService.LifetimeScope.BeginLifetimeScope();
                try
                {
                    // 替换为 Autofac Resolve，否则无法使用拦截器
                    var actor = (Actor)scope.Resolve(type.ImplementationType, TypedParameter.From(host));

                    return new State(actor, scope);
                }
                catch
                {
                    // Make sure to clean up the scope if we fail to create the actor;
                    await DisposeCore(scope);
                    throw;
                }
            }
            else
            {

                var scope = services.CreateScope();
                try
                {
                    var factory = LazyInitializer.EnsureInitialized(
                        ref this.factory,
                        ref this.initialized,
                        ref this.@lock,
                        this.initializer);

                    var actor = (Actor)factory(scope.ServiceProvider, new object[] { host });
                    return new State(actor, scope);
                }
                catch
                {
                    // Make sure to clean up the scope if we fail to create the actor;
                    await DisposeCore(scope);
                    throw;
                }
            }
        }

        public override async Task DeleteAsync(ActorActivatorState obj)
        {
            var state = (State)obj;
            await DisposeCore(state.Actor);
            await DisposeCore(state.Scope);
        }

        private async ValueTask DisposeCore(object obj)
        {
            if (obj is IAsyncDisposable asyncDisposable)
            {
                await asyncDisposable.DisposeAsync();
            }
            else if (obj is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        private class State : ActorActivatorState
        {
            public State(Actor actor, IDisposable scope)
                    : base(actor)
            {
                Scope = scope;
            }

            public IDisposable Scope { get; }
        }
    }
}
