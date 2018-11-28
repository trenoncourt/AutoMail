using System;
using System.IO;
using Automail.Api.Extensions;
using Automail.AspNetCore.Extensions;
using Automail.AspNetCore.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Automail.Api
{
    public class Program
    {
        public static void Main()
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables().Build();
            AppSettings appSettings = config.Get<AppSettings>();

            IWebHostBuilder builder = new WebHostBuilder().SuppressStatusMessages(true);
            if (appSettings?.Server?.UseIIS == true)
            {
                builder.UseIISIntegration();
            }
            
            var host = builder.UseKestrel()
                .UseSerilog((context, configuration) =>
                {
                    configuration
                        .MinimumLevel.Information()
                        .ReadFrom.Configuration(context.Configuration);
                    if (context.HostingEnvironment.IsDevelopment())
                    {
                        configuration.WriteTo.Console();
                    }
                })
                .ConfigureServices(services =>
                {
                    services.AddAutomail(config);
                    if (appSettings?.Cors?.Enabled == true)
                    {
                        services.AddCors();
                    }
                })
                .Configure(app =>
                {
                    app.ConfigureCors(appSettings);
                    app.UseAutomail();
                })
                .Build();

            host.Run();
        }
    }
}
