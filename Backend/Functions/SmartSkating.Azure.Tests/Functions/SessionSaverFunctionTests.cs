using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Sanet.SmartSkating.Backend.Functions;
using Sanet.SmartSkating.Backend.Functions.TestUtils;
using Sanet.SmartSkating.Dto;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Dto.Models.Responses;
using Sanet.SmartSkating.Dto.Services;
using Xunit;

namespace Sanet.SmartSkating.Backend.Azure.Tests.Functions
{
    public class SessionSaverFunctionTests
    {
        private readonly SessionSaverFunction _sut;
        private readonly IDataService _dataService;
        private readonly IBinder _binder = Substitute.For<IBinder>();
        private readonly List<SessionDto> _sessionsStub = new List<SessionDto>()
        {
            new SessionDto()
            {
                Id = "0",
                AccountId = "0",
                IsSaved = false,
                IsCompleted = false
            },
            new SessionDto()
            {
                Id = "1",
                AccountId = "0",
                IsSaved = false,
                IsCompleted = false
            }
        };

        public SessionSaverFunctionTests()
        {
            _dataService = Substitute.For<IDataService>();
            _sut = new SessionSaverFunction(_dataService);
        }

        [Fact]
        public async Task RunningFunctionCallsSaveSessionsForEveryItem()
        {
            await _sut.Run(Utils.CreateMockRequest(
                    _sessionsStub),
                _binder,
                Substitute.For<ILogger>());

            await _dataService.Received(2).SaveSessionAsync(Arg.Any<SessionDto>());
        }

        [Fact]
        public async Task RunningFunctionReturnsListOfSavedSessionIds()
        {
            _dataService.SaveSessionAsync(Arg.Any<SessionDto>()).ReturnsForAnyArgs(Task.FromResult(true));
        
            var actionResult = await _sut.Run(Utils.CreateMockRequest(
                    _sessionsStub),
                _binder,
                Substitute.For<ILogger>()) as JsonResult;
        
            Assert.NotNull(actionResult);
            var response = actionResult.Value as SaveEntitiesResponse;
        
            Assert.NotNull(response?.SyncedIds);
            Assert.Equal(200, response.ErrorCode);
            Assert.Equal(2, response.SyncedIds.Count);
            Assert.Equal(_sessionsStub.First().Id,response.SyncedIds.First());
            Assert.Equal(_sessionsStub.Last().Id,response.SyncedIds.Last());
        }

        [Fact]
        public async Task ReturnsServiceErrorMessage()
        {
            const string errorMessage = "some error";
            _dataService.ErrorMessage.Returns(errorMessage);
            _dataService.SaveSessionAsync(Arg.Any<SessionDto>()).ReturnsForAnyArgs(Task.FromResult(false));
        
            var actionResult = await _sut.Run(Utils.CreateMockRequest(
                    _sessionsStub),
                _binder,
                Substitute.For<ILogger>()) as JsonResult;
        
            Assert.NotNull(actionResult);
            var response = actionResult.Value as SaveEntitiesResponse;
            Assert.NotNull(response);
            Assert.Equal(errorMessage, response.Message);
        }
        
        [Fact]
        public async Task RunningFunctionWithoutProperRequestReturnsBadRequestErrorCode()
        {
            await CommonFunctionsTests.RunningFunctionWithoutProperRequestReturnsBadRequestErrorCode(_sut);
        }
        
        [Fact]
        public async Task Function_Sends_Session_To_SignalR_When_Session_Is_Closed()
        {
            _dataService.SaveSessionAsync(Arg.Any<SessionDto>()).ReturnsForAnyArgs(Task.FromResult(true));
            _sessionsStub[0].IsCompleted = true;
            
            await _sut.Run(Utils.CreateMockRequest(
                    _sessionsStub),
                _binder,
                Substitute.For<ILogger>());

            await _binder.Received(1).BindAsync<IAsyncCollector<SignalRMessage>>(new SignalRAttribute
                {HubName = ApiNames.SyncHub});
        }
        
        [Fact]
        public async Task Function_DoesNot_Send_Session_To_SignalR_When_Session_Is_NotClosed()
        {
            _dataService.SaveSessionAsync(Arg.Any<SessionDto>()).ReturnsForAnyArgs(Task.FromResult(true));
            
            await _sut.Run(Utils.CreateMockRequest(
                    _sessionsStub),
                _binder,
                Substitute.For<ILogger>());

            await _binder.DidNotReceive().BindAsync<IAsyncCollector<SignalRMessage>>(new SignalRAttribute
                {HubName = _sessionsStub[0].Id});
        }
    }
}