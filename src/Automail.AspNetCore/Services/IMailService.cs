using System.Threading.Tasks;
using Automail.AspNetCore.Dtos.Commands;

namespace Automail.AspNetCore.Services
{
    public interface IMailService
    {
        Task SendAsync(SendMailCommand mailDto, ProviderSettings settings);
    }
}