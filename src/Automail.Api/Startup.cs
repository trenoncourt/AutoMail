using Automail.Api.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Automail.Api
{
    public class Startup
    {
        private AppSettings _appSettings;
        
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public AppSettings AppSettings => _appSettings ?? (_appSettings = Configuration.Get<AppSettings>());

        public void ConfigureServices(IServiceCollection services)
        {
            if (AppSettings.Cors.Enabled)
            {
                services.AddCors();   
            }
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            }
            
            app.ConfigureCors(AppSettings);

            app.Run(async context =>
            {
                await context.Response.WriteAsync("AutoMail");
            });
        }
    }
}
