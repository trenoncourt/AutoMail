using System.Threading.Tasks;
using Automail.AspNetCore.Options;
using Flurl.Http;
using Microsoft.Extensions.Options;

namespace Automail.AspNetCore.Services
{
    public class AuthService
    {
        private readonly AzureAdOptions _azureAdOptions;

        public AuthService(IOptions<AzureAdOptions> azureAdOptions)
        {
            _azureAdOptions = azureAdOptions.Value;
        }

        public Task<Token> GetTokenAsync(AzureAdOptions azureAdOptions = null)
        {
            azureAdOptions = azureAdOptions ?? _azureAdOptions;
            return $"{azureAdOptions.Instance}{azureAdOptions.Tenant}/oauth2/token"
                .PostUrlEncodedAsync(new
                {
                    client_id = azureAdOptions.ClientId,
                    client_secret = azureAdOptions.ClientSecret,
                    grant_type = "client_credentials",
                    resource = azureAdOptions.GraphResource
                })
                .ReceiveJson<Token>();
        }
    }
}