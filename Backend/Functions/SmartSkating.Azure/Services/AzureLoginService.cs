using System.Threading.Tasks;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Dto.Services;
using Sanet.SmartSkating.Dto.Services.Account;

namespace Sanet.SmartSkating.Backend.Azure.Services
{
    public class AzureLoginService:ILoginService
    {
        private readonly IDataService _dataService;

        public AzureLoginService(IDataService dataService)
        {
            _dataService = dataService;
        }

        public async Task<AccountDto?> LoginUserAsync(string username, string password)
        {
            var sessions = await _dataService.GetAllSessionsForAccountAsync(username);
            return sessions.Count == 0 
                ? null 
                : new AccountDto
                    {
                        Id = username
                    };
        }
    }
}