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

        public Task<Token> GetTokenAsync()
        {
            return $"{_azureAdOptions.Instance}{_azureAdOptions.Tenant}/oauth2/token"
                .PostUrlEncodedAsync(new
                {
                    client_id = _azureAdOptions.ClientId,
                    client_secret = _azureAdOptions.ClientSecret,
                    grant_type = "client_credentials",
                    resource = _azureAdOptions.GraphResource
                })
                .ReceiveJson<Token>();
        }
    }
}