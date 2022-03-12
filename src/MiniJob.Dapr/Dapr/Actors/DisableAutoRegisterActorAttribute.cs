using System;

namespace MiniJob.Dapr.Actors
{
    /// <summary>
    /// 禁止自动注册Actor
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DisableAutoRegisterActorAttribute : Attribute
    {
    }
}
