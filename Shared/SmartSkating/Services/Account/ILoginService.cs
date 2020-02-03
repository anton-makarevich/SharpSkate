using System.Threading.Tasks;
using Sanet.SmartSkating.Dto.Models;

namespace Sanet.SmartSkating.Services.Account
{
    public interface ILoginService
    {
        Task<AccountDto?> LoginUserAsync(string username, string password);
    }
}