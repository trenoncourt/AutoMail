using System.Threading.Tasks;
using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace Automail.Api.Services
{
    public class MailService
    {
        private readonly AppSettings _settings;
        private readonly SmtpClient _smtpClient;
        
        public MailService(AppSettings settings)
        {
            _settings = settings;
            _smtpClient = new SmtpClient();
        }

        public async Task SendAsync(MimeMessage message)
        {
            if (!_smtpClient.IsAuthenticated || !_smtpClient.IsConnected)
            {
                _smtpClient.LocalDomain = _settings.Smtp.LocalDomain;
                await _smtpClient.ConnectAsync(_settings.Smtp.Host, _settings.Smtp.Port, _settings.Smtp.SecureSocketOptions).ConfigureAwait(false);
                if (_settings.Smtp.User != null && _settings.Smtp.Password != null)
                {
                    _smtpClient.Authenticate(_settings.Smtp.User, _settings.Smtp.Password);
                }
            }
            await _smtpClient.SendAsync(message).ConfigureAwait(false);
            if (!_settings.KeepConnection)
            {
                await _smtpClient.DisconnectAsync(true).ConfigureAwait(false);
            }
        }
    }
}