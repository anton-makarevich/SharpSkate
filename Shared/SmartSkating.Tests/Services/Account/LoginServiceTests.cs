using System.Threading.Tasks;
using NSubstitute;
using Sanet.SmartSkating.Dto.Models.Requests;
using Sanet.SmartSkating.Services.Account;
using Sanet.SmartSkating.Services.Api;
using Xunit;

namespace Sanet.SmartSkating.Tests.Services.Account
{
    public class LoginServiceTests
    {
        private readonly LoginService _sut;
        private readonly IApiService _apiService;

        public LoginServiceTests()
        {
            _apiService = Substitute.For<IApiService>();
            _sut = new LoginService(_apiService);
        }

        [Fact]
        public async Task CallsApiServiceLogin_WhenLoginAsyncIsInvoked()
        {
            const string username = "username";
            const string password = "password";
            var request = new LoginRequest
            {
                Username = username,
                Password = password
            };
            
            await _sut.LoginUserAsync(username, password);

            await _apiService.Received().LoginAsync(request, Arg.Any<string>());
        }
    }
}