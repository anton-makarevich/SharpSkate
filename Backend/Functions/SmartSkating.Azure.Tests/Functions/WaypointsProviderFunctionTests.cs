using System;
using System.Collections.Generic;
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
    public class WaypointsProviderFunctionTests
    {
        private const  string SessionId = "sessionId";
        private readonly WaypointsProviderFunction _sut;
        private readonly IDataService _dataService;
        private readonly ILogger _log;
        private readonly IBinder _binder = Substitute.For<IBinder>();
        private readonly HttpRequest _request = Utils.CreateMockRequest(queryString: $"?sessionId={SessionId}");

        public WaypointsProviderFunctionTests()
        {
            _log = Substitute.For<ILogger>();
            _dataService = Substitute.For<IDataService>();
            _sut = new WaypointsProviderFunction(_dataService);
        }

        [Fact]
        public async Task RunningFunction_Gets_Waypoints_For_Session_From_Service()
        {
            await _sut.Run(_request,_binder,_log);

            await _dataService.Received().GetWayPointForSessionAsync(SessionId);
        }

        [Fact]
        public async Task Returns_Waypoints_When_Call_To_Service_Succeeded()
        {
            var waypoints = new List<WayPointDto>
            {
                new WayPointDto()
                {
                    Id = "someId",
                    SessionId = SessionId,
                    Time = DateTime.Now,
                    DeviceId = "deviceId",
                    Coordinate = new CoordinateDto
                    {
                        Latitude = 1234,
                        Longitude = 567
                    }
                }
            };
            _dataService.GetWayPointForSessionAsync(SessionId)
                .Returns(Task.FromResult(waypoints));
            
            var actionResult = await _sut.Run(_request,_binder,_log) as JsonResult;
        
            actionResult.Should().NotBeNull();
            var response = actionResult?.Value as GetWaypointsResponse;
            response.Should().NotBeNull();
            (response?.Waypoints).Should().Equal(waypoints);
            (response?.ErrorCode).Should().Be(200);
        }
        
        [Fact]
        public async Task ReturnsBadRequestStatus_WhenRequestIsInvalid()
        {
            var request = Utils.CreateMockRequest(queryString: $"?someId={SessionId}");
            var actionResult = await _sut.Run(request,_binder,_log) as JsonResult;
        
            actionResult.Should().NotBeNull();
            var response = actionResult?.Value as GetWaypointsResponse;
            (response?.ErrorCode).Should().Be(expected: (int)HttpStatusCode.BadRequest);
        }
    }
}