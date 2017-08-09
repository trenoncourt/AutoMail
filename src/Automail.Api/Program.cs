using System;
using System.IO;
using Automail.Api.Dtos;
using Automail.Api.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
            
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureLogging(loggerFactory =>
                {
                    if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                        loggerFactory.AddConsole();
                })
                .ConfigureServices(services =>
                {
                    services.AddRouting();
                    if (appSettings.Cors.Enabled)
                    {
                        services.AddCors();
                    }
                })
                .Configure(app =>
                {
                    app.ConfigureCors(appSettings);

                    app.UseRouter(r =>
                    {
                        r.MapPost("tt", async context =>
                        {
                            var body = await context.Request.HttpContext.ReadFromJson<SendMailRequest>();
                            if (body == null) return;
 
                            await contactRepo.Add(newContact);
 
                            response.StatusCode = 201;
                            await response.WriteJson(newContact);
                        });

                        r.MapGet("contacts", async (context) => 
                        {
                            context.Response.WriteJson()
                            var contacts = await contactRepo.GetAll();
                            await response.WriteJson(contacts);
                        });
                    });
                })
                .Build();

            host.Run();
        }
    }
}
