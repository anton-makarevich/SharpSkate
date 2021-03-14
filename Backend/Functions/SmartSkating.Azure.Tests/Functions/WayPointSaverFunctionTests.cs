using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Sanet.SmartSkating.Backend.Functions;
using Sanet.SmartSkating.Backend.Functions.TestUtils;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Dto.Models.Responses;
using Sanet.SmartSkating.Dto.Models.Responses.Base;
using Sanet.SmartSkating.Dto.Services;
using Xunit;

namespace Sanet.SmartSkating.Backend.Azure.Tests.Functions
{
    public class WayPointSaverFunctionTests
    {
        private readonly WayPointSaverFunction _sut;
        private readonly IDataService _dataService;
        private readonly List<WayPointDto> _wayPointsStub = new List<WayPointDto>();
        private readonly IBinder _binder = Substitute.For<IBinder>();
        private const string SessionId = "SessionId";

        private WayPointDto GetWayPointStub(int id)
        {
            return new WayPointDto()
            {
                Coordinate = new CoordinateDto(),
                Id = id.ToString(),
                SessionId = SessionId,
                Time = DateTime.Now
            };
        }

        public WayPointSaverFunctionTests()
        {
            foreach (var i in new[]{0,1})
            {
                _wayPointsStub.Add(GetWayPointStub(i));
            }
            _dataService = Substitute.For<IDataService>();
            _sut = new WayPointSaverFunction(_dataService);
        }

        [Fact]
        public async Task RunningFunctionCallsSaveWayPointForEveryItem()
        {
            await _sut.Run(Utils.CreateMockRequest(
                    _wayPointsStub),
                _binder,
                Substitute.For<ILogger>());

            await _dataService.Received(2).SaveWayPointAsync(Arg.Any<WayPointDto>());
        }

        [Fact]
        public async Task RunningFunctionReturnsListOfSavedWayPointIds()
        {
            _dataService.SaveWayPointAsync(Arg.Any<WayPointDto>()).ReturnsForAnyArgs(Task.FromResult(true));
        
            var actionResult = await _sut.Run(Utils.CreateMockRequest(
                    _wayPointsStub),
                _binder,
                Substitute.For<ILogger>()) as JsonResult;
        
            Assert.NotNull(actionResult);
            var response = actionResult.Value as SaveEntitiesResponse;
        
            Assert.NotNull(response?.SyncedIds);
            Assert.Equal(200, response.ErrorCode);
            Assert.Equal(2, response.SyncedIds.Count);
            Assert.Equal(_wayPointsStub.First().Id,response.SyncedIds.First());
            Assert.Equal(_wayPointsStub.Last().Id,response.SyncedIds.Last());
        }
        
        [Fact]
        public async Task Function_Sends_Waypoint_To_SignalR()
        {
            _dataService.SaveWayPointAsync(Arg.Any<WayPointDto>()).ReturnsForAnyArgs(Task.FromResult(true));

            await _sut.Run(Utils.CreateMockRequest(
                    _wayPointsStub),
                _binder,
                Substitute.For<ILogger>());

            await _binder.Received(1).BindAsync<IAsyncCollector<SignalRMessage>>(new SignalRAttribute
                {HubName = SessionId});
        }
        
        [Fact]
        public async Task ReturnsServiceErrorMessage()
        {
            const string errorMessage = "some error";
            _dataService.ErrorMessage.Returns(errorMessage);
            _dataService.SaveWayPointAsync(Arg.Any<WayPointDto>()).ReturnsForAnyArgs(Task.FromResult(false));
        
            var actionResult = await _sut.Run(Utils.CreateMockRequest(
                    _wayPointsStub),
                _binder,
                Substitute.For<ILogger>()) as JsonResult;
        
            Assert.NotNull(actionResult);
            var response = actionResult.Value as SaveEntitiesResponse;
            Assert.NotNull(response);
            response.Message.Should().Contain(errorMessage);
        }
        
        [Fact]
        public async Task RunningFunctionWithoutProperRequestReturnsBadRequestErrorCode()
        {
            var actionResult = await _sut.Run(Utils.CreateMockRequest(
                    null),
                _binder,
                Substitute.For<ILogger>()) as JsonResult;
        
            Assert.NotNull(actionResult);
            var response = actionResult.Value as ResponseBase;
        
            Assert.NotNull(response);
            const int badRequestStatus = (int) HttpStatusCode.BadRequest;
            Assert.Equal(badRequestStatus, response.ErrorCode);
        }
        
        [Fact]
        public async Task ReturnsBadRequestWithAMessage_WhenTimeIsLessThanMinValueForEveryWayPoint()
        {
            foreach (var wayPointDto in _wayPointsStub)
            {
                wayPointDto.Time = DateTime.MinValue;
            }
            _dataService.SaveWayPointAsync(Arg.Any<WayPointDto>())
                .ReturnsForAnyArgs(Task.FromResult(true));
            var actionResult = await _sut.Run(Utils.CreateMockRequest(
                    _wayPointsStub),
                _binder,
                Substitute.For<ILogger>()) as JsonResult;
            
            Assert.NotNull(actionResult);
            var response = actionResult.Value as SaveEntitiesResponse;
        
            Assert.NotNull(response);
            const int badRequestStatus = (int) HttpStatusCode.BadRequest;
            response.ErrorCode.Should().Be(badRequestStatus);
            response.Message.Should().NotBeNullOrEmpty();
        }
        
        [Fact]
        public async Task ReturnsOkWithAMessage_WhenTimeIsLessThanMinValueForNotEveryWayPoint()
        {
            _wayPointsStub.First().Time = DateTime.MinValue;
            _dataService.SaveWayPointAsync(Arg.Any<WayPointDto>())
                .ReturnsForAnyArgs(Task.FromResult(true));
            
            var actionResult = await _sut.Run(Utils.CreateMockRequest(
                    _wayPointsStub),
                _binder,
                Substitute.For<ILogger>()) as JsonResult;
            
            Assert.NotNull(actionResult);
            var response = actionResult.Value as SaveEntitiesResponse;
        
            Assert.NotNull(response);
            const int okStatus = (int) HttpStatusCode.OK;
            response.ErrorCode.Should().Be(okStatus);
            response.Message.Should().NotBeNullOrEmpty();
        }
    }
}