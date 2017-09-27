using System;
using System.IO;
using Automail.Api.Dtos;
using Automail.Api.Extensions;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
                        r.MapPost("send", async context =>
                        {
                            var body = await context.Request.HttpContext.ReadFromJson<SendMailRequest>();
                            if (body == null)
                            {
                                context.Response.StatusCode = 400;
                                return;
                            }
                            var emailMessage = new MimeMessage();
                            emailMessage.From.Add(new MailboxAddress(body.FromName ?? body.From, body.From));
                            foreach(string to in body.To.Split(';'))
                                emailMessage.To.Add(new MailboxAddress("", to));
                            emailMessage.Subject = body.Subject;
                            emailMessage.Body = new TextPart(body.IsHtml ? "html" : "plain") { Text = body.Body };
                            
                            using (var client = new SmtpClient())
                            {
                                client.LocalDomain = appSettings.Smtp.LocalDomain;                
                                await client.ConnectAsync(appSettings.Smtp.Host, appSettings.Smtp.Port, appSettings.Smtp.SecureSocketOptions).ConfigureAwait(false);
                                if (appSettings.Smtp.User != null && appSettings.Smtp.Password != null)
                                {
                                    client.Authenticate(appSettings.Smtp.User, appSettings.Smtp.Password);
                                }
                                await client.SendAsync(emailMessage).ConfigureAwait(false);
                                await client.DisconnectAsync(true).ConfigureAwait(false);
                            }
                            context.Response.StatusCode = 201;
                            
                        });
                    });
                })
                .Build();

            host.Run();
        }
    }
}
