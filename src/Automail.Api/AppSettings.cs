using Automail.AspNetCore;

namespace Automail.Api
{
    public class AppSettings
    {
        
        public CorsSettings Cors { get; set; }

        public AutomailSettings Automail { get; set; }

        public ServerSettings Server { get; set; }
    }
}