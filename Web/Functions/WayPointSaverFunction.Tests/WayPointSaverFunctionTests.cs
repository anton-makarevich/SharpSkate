using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FunctionTestUtils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Dto.Models.Responses;
using Sanet.SmartSkating.Dto.Services;
using Sanet.SmartSkating.Web.Functions;
using Xunit;

namespace WayPointSaverFunctionTests
{
    public class WayPointSaverFunctionTests
    {
        private readonly WayPointSaverFunction _sut;
        private readonly IDataService _dataService;
        private readonly List<WayPointDto> _wayPointsStub = new List<WayPointDto>()
        {
            new WayPointDto()
            {
                Coordinate = new CoordinateDto(),
                Id = "0",
                SessionId = "0",
                WayPointType = "na"
            },
            new WayPointDto()
            {
                Coordinate = new CoordinateDto(),
                Id = "1",
                SessionId = "0",
                WayPointType = "na"
            }
        };

        public WayPointSaverFunctionTests()
        {
            _dataService = Substitute.For<IDataService>();
            _sut = new WayPointSaverFunction();
            _sut.SetService(_dataService);
        }

        [Fact]
        public async Task RunningFunctionCallsSavesWaypPintForEveryItem()
        {
            await _sut.Run(Utils.CreateMockRequest(
                    _wayPointsStub),
                Substitute.For<ILogger>());

            await _dataService.Received(2).SaveWayPointAsync(Arg.Any<WayPointDto>());
        }

        [Fact]
        public async Task RunningFunctionWithoutProperRequestReturnsBadRequestErrorCode()
        {
            var actionResult = await _sut.Run(Utils.CreateMockRequest(
                    null),
                Substitute.For<ILogger>()) as JsonResult;
        
            Assert.NotNull(actionResult);
            var response = actionResult.Value as SaveEntitiesResponse;
        
            Assert.NotNull(response);
            const int badRequestStatus = (int) HttpStatusCode.BadRequest;
            Assert.Equal(badRequestStatus, response.ErrorCode);
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

            Assert.Equal(errorMessage, response.Message);
        }
    }
}