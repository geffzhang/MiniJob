using Volo.Abp.DependencyInjection;

namespace MiniJob.Dapr.Security;

public class RandomStringApiTokenProvider : IDaprApiTokenProvider, ISingletonDependency
{
    public string GetAppApiToken() => Guid.NewGuid().ToString();

    public string GetDaprApiToken() => Guid.NewGuid().ToString();
}
