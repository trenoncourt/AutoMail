using System;
using Automail.AspNetCore.Options;
using Automail.AspNetCore.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Automail.AspNetCore.Extensions.DependencyInjection
{
    public static class AutomailServiceCollectionExtensions
    {
        public static IServiceCollection AddAutomail(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            services.AddRouting();
            services.AddMemoryCache();
            services.Configure<AzureAdOptions>(configuration.GetSection("AzureAd"));
            services.AddScoped<MailService>();
            services.AddScoped<MsGraphMailService>();
            services.AddScoped<AuthService>();
            services.Configure<AutomailSettings>(configuration.GetSection("Automail"));
            return services;
        }
    }
}