using System.Collections.Generic;
using MailKit.Security;

namespace Automail.AspNetCore
{
    public class AutomailSettings
    {
        public string Path { get; set; }

        public IEnumerable<ProviderSettings> Providers { get; set; }
    }

    public class CorsSettings
    {
        public bool Enabled { get; set; }

        public string Methods { get; set; }

        public string Origins { get; set; }

        public string Headers { get; set; }
    }

    public class ProviderSettings
    {
        public string Path { get; set; }

        public bool KeepConnection { get; set; }

        public SmtpSettings Smtp { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public string Tenant { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string DefaultFrom { get; set; }

        public AutomailType AutomailType
        {
            get
            {
                switch (Type)
                {
                    case "Smtp":
                        return AutomailType.Smtp;
                    case "MsGraph":
                        return AutomailType.MsGraph;
                    default:
                        return AutomailType.Smtp;
                }
            }
        }
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

    public enum AutomailType
    {
        Smtp,
        MsGraph
    }
}