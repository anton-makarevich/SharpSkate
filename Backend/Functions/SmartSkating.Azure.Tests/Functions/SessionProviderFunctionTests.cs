using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Sanet.SmartSkating.Backend.Functions;
using Sanet.SmartSkating.Backend.Functions.TestUtils;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Dto.Models.Responses;
using Sanet.SmartSkating.Dto.Services;
using Xunit;

namespace Sanet.SmartSkating.Backend.Azure.Tests.Functions
{
    public class SessionProviderFunctionTests
    {
        private const string AccountId = "accountId";
        private readonly SessionProviderFunction _sut;
        private readonly IDataService _dataService;
        private readonly ILogger _log;
        private readonly HttpRequest _request = Utils.CreateMockRequest(queryString: $"?accountId={AccountId}");
        
        public SessionProviderFunctionTests()
        {
            _log = Substitute.For<ILogger>();
            _dataService = Substitute.For<IDataService>();
            _sut = new SessionProviderFunction(_dataService);
        }

        [Fact]
        public async Task RunningFunction_Gets_Sessions_From_Service()
        {
            await _sut.Run(_request,_log);

            await _dataService.Received().GetAllSessionsForAccountAsync(AccountId);
        }

        [Fact]
        public async Task Returns_Sessions_When_call_To_Service_Succeeded()
        {
            var sessionDto = new SessionDto
            {
                Id = "sessionId",
                AccountId = AccountId,
                IsCompleted = false,
                IsSaved = true,
                RinkId = "rinkId",
                DeviceId = "deviceId"
            };
            _dataService.GetAllSessionsForAccountAsync(AccountId)
                .Returns(Task.FromResult(new List<SessionDto> {sessionDto}));
            
            var actionResult = await _sut.Run(_request,_log) as JsonResult;

            actionResult.Should().NotBeNull();
            var response = actionResult?.Value as LoginResponse;
            response?.Account.Should().Be(sessionDto);
            response?.ErrorCode.Should().Be(200);
        }

        [Fact]
        public async Task ReturnsBadRequestStatus_WhenRequestIsInvalid()
        {
            var request = Utils.CreateMockRequest(queryString: $"?someId={AccountId}");
            var actionResult = await _sut.Run(request,_log) as JsonResult;

            actionResult.Should().NotBeNull();
            var response = actionResult?.Value as LoginResponse;
            response?.ErrorCode.Should().Be(expected: (int)HttpStatusCode.BadRequest);
        }
    }
}