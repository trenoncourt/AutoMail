using System;
using System.IO;
using Automail.Api.Dtos.Requests;
using Automail.Api.Extensions;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MimeKit;

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
            if (!appSettings.IsValid())
            {
                throw new Exception("Please add all mandatory fields in settings.");
            }

            var builder = new WebHostBuilder();

            if (appSettings?.Server?.UseIIS == true)
            {
                builder.UseIISIntegration();
            }

            var host = builder.UseKestrel()
                .ConfigureLogging((hostingContext, loggerFactory) =>
                {
                    loggerFactory.AddConfiguration(config.GetSection("Logging"));
                    if (hostingContext.HostingEnvironment.IsDevelopment())
                    {
                        loggerFactory.AddConsole();
                    }
                })
                .ConfigureServices(services =>
                {
                    services.AddRouting();
                    if (appSettings?.Cors?.Enabled == true)
                    {
                        services.AddCors();
                    }
                })
                .Configure(app =>
                {
                    app.ConfigureCors(appSettings);

                    app.UseRouter(r =>
                    {
                        r.MapPost("send", async context =>
                        {
                            try
                            {
                                var body = await context.Request.HttpContext.ReadFromJson<SendMailRequest>();
                                if (body == null || !body.IsValid())
                                {
                                    context.Response.StatusCode = 400;
                                    return;
                                }
                                
                                var emailMessage = body.ToMimeMessage();
                                await emailMessage.SendAsync(appSettings);
                                context.Response.StatusCode = 204;
                            }
                            catch (Exception e)
                            {
                                context.Response.StatusCode = 500;
                                await context.Response.WriteAsync("{\"error\": \"" + e.Message + "\"}");
                            }

                        });
                    });
                })
                .Build();

            host.Run();
        }
    }
}
