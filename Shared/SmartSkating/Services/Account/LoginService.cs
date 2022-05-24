using System;
using System.Threading.Tasks;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Dto.Models.Requests;
using Sanet.SmartSkating.Dto.Services;
using Sanet.SmartSkating.Dto.Services.Account;
using Sanet.SmartSkating.Services.Api;

namespace Sanet.SmartSkating.Services.Account
{
    public class LoginService:ILoginService
    {
        private readonly IApiService _apiService;
        private readonly IConfigService _configService;

        public LoginService(IApiService apiService, IConfigService configService)
        {
            _apiService = apiService;
            _configService = configService;
        }

        public async Task<AccountDto?> LoginUserAsync(string username, string password)
        {
            var request = new LoginRequest
            {
                Username = username,
                Password = password
            };
            try
            {
                return (await _apiService.LoginAsync(request, _configService.AzureApiSubscriptionKey)).Account;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
