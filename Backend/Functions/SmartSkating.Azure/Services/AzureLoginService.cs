using System.Threading.Tasks;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Dto.Services.Account;

namespace Sanet.SmartSkating.Backend.Azure.Services
{
    public class AzureLoginService:ILoginService
    {
        public Task<AccountDto?> LoginUserAsync(string username, string password)
        {
            throw new System.NotImplementedException();
        }
    }
}