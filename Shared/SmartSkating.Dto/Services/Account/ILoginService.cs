using System.Threading.Tasks;
using Sanet.SmartSkating.Dto.Models;

namespace Sanet.SmartSkating.Dto.Services.Account
{
    public interface ILoginService
    {
        Task<AccountDto?> LoginUserAsync(string username, string password);
    }
}