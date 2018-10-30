using Automail.AspNetCore;

namespace Automail.Api
{
    public class AppSettings
    {
        
        public CorsSettings Cors { get; set; }

        public AutomailSettings Automail { get; set; }

        public ServerSettings Server { get; set; }
    }
    
    public static class AppSettingsExtensions
    {
        public static bool IsValid(this AppSettings settings)
        {
            if (settings.Automail.Providers == null) return false;
            foreach (var provider in settings.Automail.Providers)
            {
                if (string.IsNullOrEmpty(provider.Smtp.Host) || provider.Smtp.Port == 0)
                {
                    return false;
                }
            }

            return true;
        }
    }
}