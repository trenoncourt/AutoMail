using MailKit.Security;

namespace Automail.Api
{
    public class AppSettings
    {
        public CorsSettings Cors { get; set; }

        public SmtpSettings Smtp { get; set; }

        public ServerSettings Server { get; set; }

        public bool KeepConnection { get; set; }
    }

    public class CorsSettings
    {
        public bool Enabled { get; set; }

        public string Methods { get; set; }

        public string Origins { get; set; }

        public string Headers { get; set; }
    }

    public class SmtpSettings
    {
        public string LocalDomain { get; set; }

        public string Host { get; set; }

        public ushort Port { get; set; }

        public string User { get; set; }

        public string Password { get; set; }

        public string Security { get; set; }

        public SecureSocketOptions SecureSocketOptions {
            get
            {
                switch (Security)
                {
                    case "Auto":
                        return SecureSocketOptions.Auto;
                    case "Tls":
                        return SecureSocketOptions.StartTls;
                    case "Ssl":
                        return SecureSocketOptions.SslOnConnect;
                    default:
                        return SecureSocketOptions.None;
                }
            }
        }
    }

    public class ServerSettings
    {
        public bool UseIIS { get; set; }
    }
    
    public static class AppSettingsExtensions
    {
        public static bool IsValid(this AppSettings settings)
        {
            return !string.IsNullOrEmpty(settings.Smtp.Host) && settings.Smtp.Port != 0;
        }
    }
}