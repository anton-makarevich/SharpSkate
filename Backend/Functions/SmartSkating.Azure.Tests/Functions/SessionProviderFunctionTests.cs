using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
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
        private readonly IBinder _binder = Substitute.For<IBinder>();
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
            await _sut.Run(_request,_binder,_log);

            await _dataService.Received().GetAllSessionsForAccountAsync(AccountId);
        }

        [Fact]
        public async Task Returns_Sessions_When_Call_To_Service_Succeeded()
        {
            var sessions = new List<SessionDto>
            {
                new SessionDto
                {
                    Id = "sessionId",
                    AccountId = AccountId,
                    IsCompleted = false,
                    IsSaved = true,
                    RinkId = "rinkId",
                    DeviceId = "deviceId"
                }
            };
            _dataService.GetAllSessionsForAccountAsync(AccountId)
                .Returns(Task.FromResult(sessions));
            
            var actionResult = await _sut.Run(_request,_binder,_log) as JsonResult;

            actionResult.Should().NotBeNull();
            var response = actionResult?.Value as GetSessionsResponse;
            response.Should().NotBeNull();
            (response?.Sessions).Should().Equal(sessions);
            (response?.ErrorCode).Should().Be(200);
        }

        [Fact]
        public async Task ReturnsBadRequestStatus_WhenRequestIsInvalid()
        {
            var request = Utils.CreateMockRequest(queryString: $"?someId={AccountId}");
            var actionResult = await _sut.Run(request,_binder,_log) as JsonResult;

            actionResult.Should().NotBeNull();
            var response = actionResult?.Value as GetSessionsResponse;
            (response?.ErrorCode).Should().Be(expected: (int)HttpStatusCode.BadRequest);
        }
        
        [Fact]
        public async Task Returns_Only_Active_Sessions_When_Corresponding_Flag_Is_On()
        {
            var request = Utils.CreateMockRequest(queryString: $"?accountId={AccountId}&activeOnly=true");
            var activeSession = new SessionDto
            {
                Id = "sessionId",
                AccountId = AccountId,
                IsCompleted = false,
                IsSaved = true,
                RinkId = "rinkId",
                DeviceId = "deviceId"
            };
            var inActiveSession = new SessionDto
            {
                Id = "sessionId1",
                AccountId = AccountId,
                IsCompleted = true,
                IsSaved = true,
                RinkId = "rinkId",
                DeviceId = "deviceId"
            };
            _dataService.GetAllSessionsForAccountAsync(AccountId)
                .Returns(Task.FromResult(new List<SessionDto>{activeSession,inActiveSession}));
            
            var actionResult = await _sut.Run(request,_binder,_log) as JsonResult;

            actionResult.Should().NotBeNull();
            var response = actionResult?.Value as GetSessionsResponse;
            response.Should().NotBeNull();
            (response?.Sessions).Should().HaveCount(1);
            (response?.Sessions?.First()).Should().Be(activeSession);
        }
    }
}