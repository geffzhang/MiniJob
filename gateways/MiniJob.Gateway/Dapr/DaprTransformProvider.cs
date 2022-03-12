using System;
using System.Threading.Tasks;
using Yarp.ReverseProxy.Transforms;
using Yarp.ReverseProxy.Transforms.Builder;

namespace MiniJob.Dapr
{
    /// <summary>
    /// Dapr 路由转换器
    /// <para>把 /api/{appid}/{*remainder} 路由转换为 /v1.0/invoke/{appid}/method/{remainder}</para>
    /// </summary>
    public class DaprTransformProvider : ITransformProvider
    {
        public void ValidateRoute(TransformRouteValidationContext context)
        {
        }

        public void ValidateCluster(TransformClusterValidationContext context)
        {
        }

        public void Apply(TransformBuilderContext context)
        {
            // /api/{appid}/{*remainder}
            // /v1.0/invoke/{appid}/method/{remainder}
            if (context.Route.RouteId == DaprYarpConsts.ApiRoute)
            {
                context.AddRequestTransform(transformContext =>
                {
                    var prefixLength = DaprYarpConsts.DaprApiPrefix.Length + 2;
                    var index = transformContext.Path.Value!.IndexOf('/', prefixLength);
                    var appId = transformContext.Path.Value[prefixLength..index];
                    var remainder = transformContext.Path.Value[index..];
                    transformContext.ProxyRequest.RequestUri = new Uri($"{transformContext.DestinationPrefix}/v1.0/invoke/{appId}/method{remainder}");
                    return ValueTask.CompletedTask;
                });
            }
        }
    }
}