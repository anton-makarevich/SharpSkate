using System;
using System.Threading.Tasks;
using Sanet.SmartSkating.Dto;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Dto.Models.Requests;
using Sanet.SmartSkating.Dto.Services.Account;
using Sanet.SmartSkating.Services.Api;

namespace Sanet.SmartSkating.Services.Account
{
    public class LoginService:ILoginService
    {
        private readonly IApiService _apiService;

        public LoginService(IApiService apiService)
        {
            _apiService = apiService;
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
                return (await _apiService.LoginAsync(request, ApiNames.AzureApiSubscriptionKey))?.Account;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}