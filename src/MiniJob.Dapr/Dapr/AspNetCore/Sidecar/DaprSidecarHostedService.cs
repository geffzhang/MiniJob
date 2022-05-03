using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MiniJob.Dapr.AspNetCore.Metrics;

namespace MiniJob.Dapr.AspNetCore.Sidecar;

public class DaprSidecarHostedService : DaprHostedService<IDaprSidecarHost, DaprSidecarOptions>
{
    private readonly IServiceProvider _serviceProvider;

    public DaprSidecarHostedService(
        IDaprSidecarHost daprSidecarHost,
        IOptionsMonitor<DaprOptions> optionsAccessor,
        IServiceProvider serviceProvider = null)
        : base(daprSidecarHost, optionsAccessor)
    {
        _serviceProvider = serviceProvider;
    }

    protected override void OnStarting(DaprOptions options, CancellationToken cancellationToken)
    {
        WaitForApplicationStart(cancellationToken);

        // If cancelled then exit
        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        // Assign metrics
        options.Sidecar ??= new DaprSidecarOptions();
        options.Sidecar.Metrics ??= new DaprMetricsOptions();
        options.Sidecar.Metrics.SetLabel(DaprMetricsConstants.ServiceLabelName, options.Sidecar.AppId);
        options.Sidecar.Metrics.SetLabel(DaprMetricsConstants.AppLabelName, DaprMetricsConstants.DaprSidecarLabel);

        // 如果未定义应用程序端口，则从 ASP.NET Core 获取
        // 如果需要，这将在等待主机启动时阻塞
        if (options.Sidecar.AppPort == null)
        {
            // 尝试从主机服务器提取端口
            var server = _serviceProvider?.GetService<IServer>();
            if (server != null)
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    // 获取所有服务器地址并解析为Uris
                    var serverAddresses = server.Features?
                        .Get<Microsoft.AspNetCore.Hosting.Server.Features.IServerAddressesFeature>()?
                        .Addresses?
                        .Select(x => Parse(x))
                        .Where(x => x != null)
                        .ToArray()
                        ?? Array.Empty<Uri>();

                    if (serverAddresses.Length > 0)
                    {
                        // 如果指定了AppSsl，则根据 scheme 找到最佳匹配
                        // 默认为第一个条目
                        var selectedAddress = serverAddresses[0];
                        foreach (var address in serverAddresses)
                        {
                            if ((options.Sidecar.AppSsl == true && address.Scheme == Uri.UriSchemeHttps) ||
                                (options.Sidecar.AppSsl == false && address.Scheme == Uri.UriSchemeHttp))
                            {
                                selectedAddress = address;
                                break;
                            }
                            else if (address.Scheme == Uri.UriSchemeHttp)
                            {
                                selectedAddress = address;
                            }
                        }

                        // 地址找到，设置端口
                        options.Sidecar.AppPort = selectedAddress.Port;
                        options.Sidecar.AppSsl = selectedAddress.Scheme == Uri.UriSchemeHttps;
                        break;
                    }

                    // 等待 hosting 启动
                    Thread.Sleep(10);
                }
            }
        }
    }

    protected virtual void WaitForApplicationStart(CancellationToken cancellationToken)
    {
        var logger = _serviceProvider?.GetService<ILogger<DaprSidecarHostedService>>();
        var applicationLifetime = _serviceProvider?.GetService<IHostApplicationLifetime>();
        if (applicationLifetime == null)
        {
            // Not yet started, log out a waiting message
            logger?.LogInformation("Host application lifetime tracking not available, starting Dapr process");

            // Either no lifetime tracker, or no service provider so cannot wait for startup
            return;
        }

        // Trigger the wait handle when the application has started to release Dapr startup process.
        var waitForApplicationStart = new ManualResetEventSlim();
        applicationLifetime.ApplicationStarted.Register(() => waitForApplicationStart.Set());
        if (applicationLifetime.ApplicationStarted.IsCancellationRequested)
        {
            logger?.LogInformation("Host application ready, starting Dapr process");

            // Started token has already triggered. No need to wait.
            return;
        }

        // Ensure the host has started
        while (!cancellationToken.IsCancellationRequested)
        {
            // Not yet started, log out a waiting message
            logger?.LogInformation("Host application is initializing, waiting to start Dapr process...");

            // Wait for the host to start so all configurations, environment variables
            // and ports are fully initialized.
            if (waitForApplicationStart.Wait(TimeSpan.FromSeconds(1), cancellationToken))
            {
                break;
            }
        }

        // Not yet started, log out a waiting message
        logger?.LogInformation("Host application ready, starting Dapr process");
    }

    public static Uri Parse(string uri) =>
            uri.IsNullOrEmpty() ? null :
            Uri.TryCreate(AdjustUriHost(uri), UriKind.Absolute, out var result) ? result :
            null;

    private static string AdjustUriHost(string uri)
    {
        if (uri.Contains("*:"))
        {
            return uri.Replace("*:", $"{DaprConstants.LocalhostAddress}:");
        }
        else if (uri.Contains("+:"))
        {
            return uri.Replace("+:", $"{DaprConstants.LocalhostAddress}:");
        }
        else
        {
            return uri;
        }
    }
}
