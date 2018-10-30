using System;
using Automail.AspNetCore.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Automail.AspNetCore.Extensions.DependencyInjection
{
    public static class AutomailServiceCollectionExtensions
    {
        public static IServiceCollection AddAutomail(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            services.AddRouting();
            services.AddScoped<MailService>();
            services.Configure<AutomailSettings>(configuration.GetSection("Automail"));
            return services;
        }
    }
}