using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using FunctionTestUtils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Dto.Models.Responses;
using Sanet.SmartSkating.Dto.Services;
using Sanet.SmartSkating.Web.Functions;
using Xunit;

namespace Sanet.SmartSkating.Azure.Tests.Functions
{
    public class WayPointSaverFunctionTests
    {
        private readonly WayPointSaverFunction _sut;
        private readonly IDataService _dataService;
        private readonly List<WayPointDto> _wayPointsStub = new List<WayPointDto>();

        private WayPointDto GetWayPointStub(int id)
        {
            return new WayPointDto()
            {
                Coordinate = new CoordinateDto(),
                Id = id.ToString(),
                SessionId = "0",
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
            _sut = new WayPointSaverFunction();
            _sut.SetService(_dataService);
        }

        [Fact]
        public async Task RunningFunctionCallsSaveWayPointForEveryItem()
        {
            await _sut.Run(Utils.CreateMockRequest(
                    _wayPointsStub),
                Substitute.For<ILogger>());

            await _dataService.Received(2).SaveWayPointAsync(Arg.Any<WayPointDto>());
        }

        [Fact]
        public async Task RunningFunctionReturnsListOfSavedWayPointIds()
        {
            _dataService.SaveWayPointAsync(Arg.Any<WayPointDto>()).ReturnsForAnyArgs(Task.FromResult(true));
        
            var actionResult = await _sut.Run(Utils.CreateMockRequest(
                    _wayPointsStub),
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
        public async Task ReturnsServiceErrorMessage()
        {
            const string errorMessage = "some error";
            _dataService.ErrorMessage.Returns(errorMessage);
            _dataService.SaveWayPointAsync(Arg.Any<WayPointDto>()).ReturnsForAnyArgs(Task.FromResult(false));
        
            var actionResult = await _sut.Run(Utils.CreateMockRequest(
                    _wayPointsStub),
                Substitute.For<ILogger>()) as JsonResult;
        
            Assert.NotNull(actionResult);
            var response = actionResult.Value as SaveEntitiesResponse;
            Assert.NotNull(response);
            response.Message.Should().Contain(errorMessage);
        }
        
        [Fact]
        public async Task RunningFunctionWithoutProperRequestReturnsBadRequestErrorCode()
        {
            await CommonFunctionsTests.RunningFunctionWithoutProperRequestReturnsBadRequestErrorCode(_sut);
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