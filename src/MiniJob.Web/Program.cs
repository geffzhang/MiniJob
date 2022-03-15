using MiniJob.Web;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
#if DEBUG
    .MinimumLevel.Debug()
#else
    .MinimumLevel.Information()
#endif
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Async(c => c.File("Logs/logs.txt", rollingInterval: RollingInterval.Day))
#if DEBUG
    .WriteTo.Async(c => c.Console())
#endif
    .ReadFrom.Configuration(builder.Configuration, "Serilog")
    .CreateLogger();

try
{
    Log.Information("Starting web host.");

    builder.Host.AddAppSettingsSecretsJson()
        .UseAutofac()
        .UseSerilog();
    await builder.AddApplicationAsync<MiniJobWebModule>();
    var app = builder.Build();
    await app.InitializeApplicationAsync();
    await app.RunAsync("http://localhost:9999");
    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly!");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}