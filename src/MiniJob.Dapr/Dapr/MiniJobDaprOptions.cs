using System;

namespace MiniJob.Dapr
{
    /// <summary>
    /// Dapr 配置选项
    /// </summary>
    public class MiniJobDaprOptions
    {
        /// <summary>
        /// 配置节名称
        /// </summary>
        public const string Dapr = "Dapr";

        /// <summary>
        /// 是否启用Dapr Sidecar，默认不启用，建议只在开发环境启用
        /// </summary>
        public bool RunSidecar { get; set; } = false;

        /// <summary>
        /// -app-id: 用于服务发现的应用程序 Id, 默认为 minijob
        /// </summary>
        public string AppId { get; set; } = "minijob";

        /// <summary>
        /// -app-max-concurrency: 应用程序的并发级别，默认为无限制
        /// </summary>
        public int AppMaxConcurrency { get; set; } = -1;

        /// <summary>
        /// -app-port: 应用程序正在侦听的端口，无需设置，自动检测
        /// </summary>
        public int AppPort { get; set; }

        /// <summary>
        /// -app-protocol: 协议（gRPC 或 HTTP） Dapr 用于与应用程序通信, 默认为 http
        /// </summary>
        public string AppProtocol { get; set; } = "http";

        /// <summary>
        /// -app-ssl: 是否https，无需设置，自动检测
        /// </summary>
        public bool AppSSl { get; set; }

        /// <summary>
        /// -components-path: Components 目录的路径，为空则使用默认路径
        /// <para>Linux、Mac: $HOME/.dapr/components</para>
        /// <para>Windows: %USERPROFILE%\.dapr\components</para>
        /// </summary>
        public string ComponentsPath { get; set; }

        /// <summary>
        /// -config: Dapr 配置文件，为空则使用默认配置
        /// <para>Linux、Mac: $HOME/.dapr/config.yaml</para>
        /// <para>Windows: %USERPROFILE%\.dapr\config.yaml</para>
        /// </summary>
        public string Config { get; set; }

        /// <summary>
        /// -dapr-grpc-port: Dapr 要监听的 gRPC 端口，默认为 50001
        /// </summary>
        public int DaprGrpcPort { get; set; } = 50001;

        /// <summary>
        /// -dapr-http-port: Dapr 要监听的 HTTP 端口，默认为 35000
        /// </summary>
        public int DaprHttpPort { get; set; } = 35000;

        public string DaprApiToken { get; set; }

        /// <summary>
        /// -enable-metrics: 是否开启 Metric 端点，默认为 false
        /// </summary>
        public bool EnableMetrics { get; set; } = false;

        /// <summary>
        /// -log-level: 日志详细程度。有效值: debug, info, warn, error, fatal，默认为 info
        /// </summary>
        public string LogLevel { get; set; } = "info";

        /// <summary>
        /// -placement-host-address: Placement 服务所在的主机地址，默认为 localhost:6050
        /// </summary>
        public string PlacementHostAddress { get; set; } = "localhost:6050";

        /// <summary>
        /// 根据配置生成运行脚本
        /// </summary>
        /// <returns></returns>
        public string GenerateScript()
        {
            if (string.IsNullOrWhiteSpace(AppId))
                throw new ArgumentNullException(nameof(AppId));

            var arguments = $"daprd " +
                $"-app-id={AppId} " +
                $"-app-protocol={AppProtocol} " +
                $"-dapr-grpc-port={DaprGrpcPort} " +
                $"-dapr-http-port={DaprHttpPort} " +
                $"-log-level={LogLevel} " +
                $"-placement-host-address={PlacementHostAddress} " +
                $"-enable-metrics={EnableMetrics} ";

            if (AppMaxConcurrency > 0)
                arguments += $" -app-max-concurrency={AppMaxConcurrency}";
            if (!string.IsNullOrWhiteSpace(ComponentsPath))
                arguments += $" -components-path={ComponentsPath}";
            if (!string.IsNullOrWhiteSpace(Config))
                arguments += $" -config={Config}";

            return arguments;
        }
    }
}
