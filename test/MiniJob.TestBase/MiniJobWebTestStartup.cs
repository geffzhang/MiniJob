using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MiniJob;

public class MiniJobWebTestStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddApplication<MiniJobTestBaseModule>();
    }

    public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
    {
        app.InitializeApplication();
    }
}
