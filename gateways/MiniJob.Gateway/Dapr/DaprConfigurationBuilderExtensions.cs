using Microsoft.Extensions.Options;
using MiniJob.Dapr;

namespace Microsoft.Extensions.Configuration;

public static class DaprConfigurationBuilderExtensions
{
    /// <summary>
    /// 添加Dapr配置
    /// <para>把 /api/{appid}/{*remainder} 路由转换为 /v1.0/invoke/{appid}/method/{remainder}</para>
    /// </summary>
    /// <param name="reverseProxyBuilder"></param>
    /// <param name="configurationBuilder"></param>
    /// <returns></returns>
    public static IReverseProxyBuilder LoadDaprConfig(
        this IReverseProxyBuilder reverseProxyBuilder,
        WebApplicationBuilder builder)
    {
        var httpEndpoint = DaprDefaults.GetDefaultHttpEndpoint();

        builder.Services.Configure<MiniJobDaprOptions>(builder.Configuration.GetSection(MiniJobDaprOptions.Dapr));
        var options = new MiniJobDaprOptions();
        builder.Services.GetSingletonInstanceOrNull<IConfigureOptions<MiniJobDaprOptions>>()
            .Configure(options);

        if (options.RunSidecar)
            httpEndpoint = $"http://127.0.0.1:{options.DaprHttpPort}";

        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string>
        {
            [$"{DaprYarpConsts.SectionKey}:Clusters:{DaprYarpConsts.DaprSideCarCluster}:Destinations:dapr-cluster/destination:Address"] =
                httpEndpoint,
            [$"{DaprYarpConsts.SectionKey}:Routes:{DaprYarpConsts.ApiRoute}:ClusterId"] = DaprYarpConsts.DaprSideCarCluster,
            [$"{DaprYarpConsts.SectionKey}:Routes:{DaprYarpConsts.ApiRoute}:Match:Path"] = "/" + DaprYarpConsts.DaprApiPrefix + "/{**catch-all}"
        });

        return reverseProxyBuilder.AddTransforms<DaprTransformProvider>();
    }

    public static T GetSingletonInstanceOrNull<T>(this IServiceCollection services)
    {
        return (T)services
            .FirstOrDefault(d => d.ServiceType == typeof(T))
            ?.ImplementationInstance;
    }
}