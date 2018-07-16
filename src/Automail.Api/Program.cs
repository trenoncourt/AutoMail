using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Automail.Api.Dtos.Requests;
using Automail.Api.Extensions;
using MailKit;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MimeKit;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using ILogger = Microsoft.Extensions.Logging.ILogger;

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

            IWebHostBuilder builder = new WebHostBuilder().SuppressStatusMessages(true);
            if (appSettings?.Server?.UseIIS == true)
            {
                builder.UseIISIntegration();
            }

            var host = builder.UseKestrel()
//                .ConfigureLogging((hostingContext, loggerFactory) =>
//                {
//                    loggerFactory.AddConfiguration(config.GetSection("Logging"));
//                    if (hostingContext.HostingEnvironment.IsDevelopment())
//                    {
//                        loggerFactory.AddConsole();
//                    }
//                })
                .UseSerilog((context, configuration) =>
                {
                    configuration
                            .MinimumLevel.Information()
                        .ReadFrom.Configuration(context.Configuration)
                        .MinimumLevel.Override("Microsoft", LogEventLevel.Fatal);
                    if (appSettings?.WriteToMsSqlServer == true)
                    {
                        // TODO: wait for next release of serilog-sinks-mssqlserver to get configuration config
                        // ref: https://github.com/serilog/serilog-sinks-mssqlserver
                        var columnOptions = new ColumnOptions();
                        columnOptions.Store.Remove(StandardColumn.MessageTemplate);
                        columnOptions.Store.Remove(StandardColumn.Properties);
                        columnOptions.Store.Remove(StandardColumn.Message);
                        columnOptions.Store.Remove(StandardColumn.Level);
                        columnOptions.TimeStamp.ColumnName = "TimeStamp";
                        columnOptions.TimeStamp.ConvertToUtc = true;
                        columnOptions.Exception.ColumnName = "Exception";
                        columnOptions.AdditionalDataColumns = new List<DataColumn>
                        {
                            new DataColumn("From", typeof(string)),
                            new DataColumn("To", typeof(string)),
                            new DataColumn("Cc", typeof(string)),
                            new DataColumn("Subject", typeof(string)),
                            new DataColumn("Body", typeof(string))
                        };
                        
                        configuration.WriteTo.MSSqlServer(appSettings.ConnectionString, "Logs",
                            columnOptions: columnOptions, autoCreateSqlTable: true);
                    }
                    if (context.HostingEnvironment.IsDevelopment())
                    {
                        configuration.WriteTo.Console();
                    }
                })
                .ConfigureServices(services =>
                {
                    services
                        .AddRouting()
                        .AddSingleton(appSettings)
                        .AddScoped<Services.MailService>();
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
                            ILogger logger = context.RequestServices.GetService<ILogger<Program>>();
                            try
                            {
                                var body = await context.Request.HttpContext.ReadFromJson<SendMailRequest>();
                                if (body == null || !body.IsValid(appSettings))
                                {
                                    context.Response.StatusCode = 400;
                                    return;
                                }

                                var emailMessage = body.ToMimeMessage(appSettings);
                                var mailService = context.RequestServices.GetService<Services.MailService>();
                                await mailService.SendAsync(emailMessage);
                                logger.LogInformation("mail sent: {From} {To} {Cc} {Subject} {Body}", body.From, body.To, body.Cc, body.Subject, body.Body);
                                context.Response.StatusCode = 204;
                            }
                            catch (Exception e)
                            {
                                context.Response.StatusCode = 500;
                                logger.LogError(e, e.Message);
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
