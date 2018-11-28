using System;
using Automail.AspNetCore.Dtos.Commands;
using Automail.AspNetCore.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Automail.AspNetCore.Extensions
{
    public static class AutomailApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseAutomail(this IApplicationBuilder builder)
        {
            var settings = builder.ApplicationServices.GetService<IOptions<AutomailSettings>>().Value;

            return builder.UseRouter(r =>
            {
                if (settings?.Providers == null) return;
                
                foreach (var provider in settings?.Providers)
                {
                    string defaultPath = settings.Path ?? "";
                    string basePath = string.IsNullOrEmpty(provider.Path) ? defaultPath : $"{defaultPath}/{provider.Path}/";
                    r.MapPost($"{basePath}send", async context =>
                    {
                        var loggerFactory = context.RequestServices.GetService<ILoggerFactory>();
                        ILogger logger = loggerFactory.CreateLogger("AutomailApplicationBuilderExtensions");
                        try
                        {
                            if (!context.HasAuthorizedContentType())
                            {
                                context.Response.StatusCode = 400;
                                return;
                            }

                            SendMailCommand body;
                            if (context.Request.HasFormContentType)
                            {
                                body = JsonConvert.DeserializeObject<SendMailCommand>(context.Request.Form["data"].ToString());
                                body.Files = context.Request.Form.Files;
                            }
                            else
                            {
                                body = await context.Request.HttpContext.ReadFromJson<SendMailCommand>();
                            }
                            if (body == null || !body.IsValid(provider.Smtp))
                            {
                                context.Response.StatusCode = 400;
                                return;
                            }

                            IMailService mailService;
                            if (provider.AutomailType == AutomailType.MsGraph)
                            {
                                mailService = context.RequestServices.GetService<MsGraphMailService>();
                            } 
                            else
                            {
                                mailService = context.RequestServices.GetService<MailService>();
                            }
                             
                            await mailService.SendAsync(body, provider);
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
                }
            });
        }
    }
}