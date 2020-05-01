using System;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
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
        private const string Username = "username";
        private const string Password = "password";

        public LoginServiceTests()
        {
            _apiService = Substitute.For<IApiService>();
            _sut = new LoginService(_apiService);
        }

        [Fact]
        public async Task CallsApiServiceLogin_WhenLoginAsyncIsInvoked()
        {
            var request = new LoginRequest
            {
                Username = Username,
                Password = Password
            };
            
            await _sut.LoginUserAsync(Username, Password);

            await _apiService.Received().LoginAsync(request, Arg.Any<string>());
        }

        [Fact]
        public async Task LoginMethodReturnsNull_WhenApiExceptionOccurs()
        {
            _apiService.LoginAsync(Arg.Any<LoginRequest>(), Arg.Any<string>())
                .ThrowsForAnyArgs(new Exception());

            var account = await _sut.LoginUserAsync(Username,Password);

            account.Should().BeNull();
        } 
    }
}