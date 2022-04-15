using Volo.Abp.DependencyInjection;

namespace MiniJob.Dapr.Security;

public class DaprApiTokenProvider : IDaprApiTokenProvider, ISingletonDependency
{
    public string GetAppApiToken() => Guid.NewGuid().ToString();

    public string GetDaprApiToken() => Guid.NewGuid().ToString();
}
