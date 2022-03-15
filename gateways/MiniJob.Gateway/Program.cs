using Microsoft.Extensions.Options;
using MiniJob.Dapr;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
#if DEBUG
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Information)
#else
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
#endif
    .Enrich.FromLogContext()
    .WriteTo.Async(c => c.File("Logs/logs.txt", rollingInterval: RollingInterval.Day))
#if DEBUG
    .WriteTo.Async(c => c.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3} {SourceContext}] {Message:lj}{NewLine}{Exception}"))
#endif
    .ReadFrom.Configuration(builder.Configuration, "Serilog")
    .CreateLogger();

try
{
    Log.Information("Starting MiniJobGateway Host...");

    builder.Host.UseSerilog();
    builder.Services.AddReverseProxy()
        .LoadDaprConfig(builder)
        .LoadFromConfig(builder.Configuration.GetSection("Yarp"));

    var app = builder.Build();

    app.UseHttpsRedirection();

    app.UseRouting();
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapReverseProxy();
    });

    app.MapGet("/", () => "Hello MiniJob Gateway!");

    app.Lifetime.ApplicationStarted.Register(() =>
    {
        var options = app.Services.GetRequiredService<IOptions<MiniJobDaprOptions>>().Value;
        options.RunDaprSidecar(app.Urls.FirstOrDefault());
    });

    app.Run("http://localhost:9000");
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly!");
}
finally
{
    Log.CloseAndFlush();
}