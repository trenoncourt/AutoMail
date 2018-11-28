using System.Threading.Tasks;
using Automail.AspNetCore.Dtos.Commands;
using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace Automail.AspNetCore.Services
{
    public class MailService : IMailService
    {
        private readonly SmtpClient _smtpClient;
        
        public MailService()
        {
            _smtpClient = new SmtpClient();
        }

        public async Task SendAsync(SendMailCommand mailDto, ProviderSettings settings)
        {
            var message = mailDto.ToMimeMessage(settings.Smtp);
            if (!_smtpClient.IsAuthenticated || !_smtpClient.IsConnected)
            {
                _smtpClient.LocalDomain = settings.Smtp.LocalDomain;
                await _smtpClient.ConnectAsync(settings.Smtp.Host, settings.Smtp.Port, settings.Smtp.SecureSocketOptions).ConfigureAwait(false);
                if (settings.Smtp.User != null && settings.Smtp.Password != null)
                {
                    _smtpClient.Authenticate(settings.Smtp.User, settings.Smtp.Password);
                }
            }
            await _smtpClient.SendAsync(message).ConfigureAwait(false);
            if (!settings.KeepConnection)
            {
                await _smtpClient.DisconnectAsync(true).ConfigureAwait(false);
            }
        }
    }
}