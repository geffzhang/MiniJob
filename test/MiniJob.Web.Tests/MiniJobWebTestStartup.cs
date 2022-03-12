using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Volo.Abp;

namespace MiniJob
{
    public class MiniJobWebTestStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplication<MiniJobWebTestModule>();
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            app.InitializeApplication();
        }
    }
}