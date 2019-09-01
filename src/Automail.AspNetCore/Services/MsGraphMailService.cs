using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Automail.AspNetCore.Dtos.Commands;
using Automail.AspNetCore.Options;
using Flurl.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MimeKit;
using Newtonsoft.Json;

namespace Automail.AspNetCore.Services
{
    public class MsGraphMailService : IMailService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly AuthService _authService;
        private readonly AzureAdOptions _azureAdOptions;

        public MsGraphMailService(IMemoryCache memoryCache, AuthService authService, IOptions<AzureAdOptions> azureAdOptions)
        {
            _memoryCache = memoryCache;
            _authService = authService;
            _azureAdOptions = azureAdOptions.Value;
        }   
        
        public async Task SendAsync(SendMailCommand mailDto, ProviderSettings settings)
        {
            string cacheTokenName = $"{settings.Name ?? ""}_token";
            var token = _memoryCache.Get<Token>(cacheTokenName);
            
            if (token == null || token.HasExpired)
            {
                token = await _authService.GetTokenAsync(new AzureAdOptions
                {
                    Tenant = settings.Tenant,
                    ClientId = settings.ClientId,
                    ClientSecret = settings.ClientSecret,
                    Instance = "https://login.microsoftonline.com/",
                    GraphResource = "https://graph.microsoft.com"
                });
                _memoryCache.Set(cacheTokenName, token);
            }

            string from = mailDto.From ?? settings.DefaultFrom;
            if (string.IsNullOrEmpty(from))
            {
                throw new Exception("From is required");
            }
            
            await $"{_azureAdOptions.GraphResource}/beta/users/{from}/sendMail"
                .WithOAuthBearerToken(token.AccessToken)
                .PostJsonAsync(new
                {
                    message = new {
                    subject = mailDto.Subject,
                    body = new
                    {
                        contentType = mailDto.IsHtml ? "HTML" : "Text",
                        content = mailDto.Body
                    },
                    toRecipients = mailDto.To?.Split(new [] {';'}, StringSplitOptions.RemoveEmptyEntries)
                        .Select(v => new
                        {
                            emailAddress = new
                            {
                                address = v
                            }
                        }),
                    ccRecipients = (IEnumerable) mailDto.Cc?.Split(new [] {';'}, StringSplitOptions.RemoveEmptyEntries)
                                       .Select(v => new
                                       {
                                           emailAddress = new
                                           {
                                               address = v
                                           }
                                       }) ?? new string[] {}
                    },
                    saveToSentItems = "false"
                });
        }
    }
    
    public class Token
    {
        private double _expiresIn;
        
        [JsonProperty(PropertyName = "expires_in")]
        public double ExpiresIn {
            get => _expiresIn;
            set
            {
                ExpireOn = DateTime.Now.AddSeconds(value);
                _expiresIn = value;
            } }

        [JsonProperty(PropertyName = "access_token")]
        public string AccessToken { get; set; }

        [JsonIgnore]
        public DateTime ExpireOn { get; set; }
        
        [JsonIgnore]
        public bool HasExpired => DateTime.Now >= ExpireOn;
    }
}