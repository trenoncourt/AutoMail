using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http;
using MimeKit;

namespace Automail.Api.Extensions
{
    public static class MailExtensions
    {
        public static async Task SendAsync(this MimeMessage mailMessage, AppSettings settings)
        {
            using (var client = new SmtpClient())
            {
                client.LocalDomain = settings.Smtp.LocalDomain;
                await client.ConnectAsync(settings.Smtp.Host, settings.Smtp.Port, settings.Smtp.SecureSocketOptions).ConfigureAwait(false);
                if (settings.Smtp.User != null && settings.Smtp.Password != null)
                {
                    client.Authenticate(settings.Smtp.User, settings.Smtp.Password);
                }
                await client.SendAsync(mailMessage).ConfigureAwait(false);
                await client.DisconnectAsync(true).ConfigureAwait(false);
            }
        }
    }
}