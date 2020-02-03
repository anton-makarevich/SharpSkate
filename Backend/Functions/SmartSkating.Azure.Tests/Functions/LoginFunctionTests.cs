using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Sanet.SmartSkating.Backend.Functions;
using Sanet.SmartSkating.Backend.Functions.TestUtils;
using Sanet.SmartSkating.Dto.Models.Requests;
using Sanet.SmartSkating.Dto.Services.Account;
using Xunit;

namespace Sanet.SmartSkating.Backend.Azure.Tests.Functions
{
    public class LoginFunctionTests
    {
        private readonly LoginFunction _sut;
        private readonly ILoginService _loginService;
        
        private readonly LoginRequest _loginStub = new LoginRequest
        {
            Username = "username",
            Password = "password"
        };

        public LoginFunctionTests()
        {
            _loginService = Substitute.For<ILoginService>();
            _sut = new LoginFunction();
            _sut.SetService(_loginService);
        }

        [Fact]
        public async Task RunningFunctionCallsSaveDevice()
        {
            await _sut.Run(Utils.CreateMockRequest(
                    _loginStub),
                Substitute.For<ILogger>());

            await _loginService.Received().LoginUserAsync(_loginStub.Username,_loginStub.Password);
        }
    }
}