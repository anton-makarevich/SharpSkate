using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Sanet.SmartSkating.Backend.Functions;
using Sanet.SmartSkating.Backend.Functions.TestUtils;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Dto.Models.Requests;
using Sanet.SmartSkating.Dto.Models.Responses;
using Sanet.SmartSkating.Dto.Services.Account;
using Xunit;

namespace Sanet.SmartSkating.Backend.Azure.Tests.Functions
{
    public class LoginFunctionTests
    {
        private readonly LoginFunction _sut;
        private readonly ILoginService _loginService;
        private readonly ILogger _log;
        private readonly IBinder _binder = Substitute.For<IBinder>();
        
        private readonly LoginRequest _loginStub = new LoginRequest
        {
            Username = "username",
            Password = "password"
        };

        public LoginFunctionTests()
        {
            _log = Substitute.For<ILogger>();
            _loginService = Substitute.For<ILoginService>();
            _sut = new LoginFunction(_loginService);
        }

        [Fact]
        public async Task RunningFunctionCallsSaveDevice()
        {
            await _sut.Run(Utils.CreateMockRequest(
                    _loginStub),_binder,_log
                );

            await _loginService.Received().LoginUserAsync(_loginStub.Username,_loginStub.Password);
        }

        [Fact]
        public async Task ReturnsAccountInfo_WhenLoginIsSuccessful()
        {
            var account = new AccountDto
            {
                Id = _loginStub.Username
            };
            _loginService.LoginUserAsync(_loginStub.Username, _loginStub.Password)
                .Returns(Task.FromResult(account));
            
            var actionResult = await _sut.Run(Utils.CreateMockRequest(
                    _loginStub),_binder,_log) as JsonResult;

            actionResult.Should().NotBeNull();
            var response = actionResult?.Value as LoginResponse;
            response?.Account.Should().Be(account);
            response?.ErrorCode.Should().Be(200);
        }

        [Fact]
        public async Task ReturnsBadRequestStatus_WhenRequestIsInvalid()
        {
            var actionResult = await _sut.Run(Utils.CreateMockRequest(
                    null),_binder,_log) as JsonResult;

            actionResult.Should().NotBeNull();
            var response = actionResult?.Value as LoginResponse;
            response?.ErrorCode.Should().Be(expected: (int)HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task ReturnsNotFound_WhenLoginIsNotSuccessful()
        {
            _loginService.LoginUserAsync(_loginStub.Username, _loginStub.Password)
                .Returns(Task.FromResult<AccountDto>(null));
            var actionResult = await _sut.Run(Utils.CreateMockRequest(
                    _loginStub),_binder,_log) as JsonResult;
            
            actionResult.Should().NotBeNull();
            var response = actionResult?.Value as LoginResponse;
            response?.ErrorCode.Should().Be(expected: (int)HttpStatusCode.NotFound);
        }
    }
}