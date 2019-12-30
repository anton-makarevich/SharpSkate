using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Dto.Models.Responses;
using Sanet.SmartSkating.Dto.Services;
using Sanet.SmartSkating.Services.Api;
using Xunit;

namespace Sanet.SmartSkating.Tests.Services.Api
{
    public class DataSyncServiceTests
    {
        private readonly DataSyncService _sut;
        private readonly IDataService _dataService;
        private readonly IApiService _apiService;
        private readonly IConnectivityService _connectivityService;
        private readonly List<WayPointDto> _wayPoints = new List<WayPointDto>
        {
            new WayPointDto
            {
                Coordinate = new CoordinateDto {Latitude = 23, Longitude = 34},
                Id = "0",
                SessionId = "0",
                Time = DateTime.Now,
                WayPointType = "na"
            }
        };

        public DataSyncServiceTests()
        {
            _dataService = Substitute.For<IDataService>();
            _apiService = Substitute.For<IApiService>();
            _connectivityService = Substitute.For<IConnectivityService>();
            
            _sut = new DataSyncService(_dataService,_apiService,_connectivityService);
        }

        [Fact]
        public async Task WhenStartedAndHasConnectionReadsLocalWayPointsAndSendThemToApiIfFound()
        {
            _connectivityService.IsConnected().Returns(Task.FromResult(true));
            _dataService.GetAllWayPointsAsync().Returns(Task.FromResult(_wayPoints));
            
            _sut.StartSyncing();

            await _apiService.Received().PostWaypointsAsync(_wayPoints);
        }
        
        [Fact]
        public async Task DoesNotStartSyncServiceSecondTime()
        {
            _connectivityService.IsConnected().Returns(Task.FromResult(true));
            _dataService.GetAllWayPointsAsync().Returns(Task.FromResult(_wayPoints));
            
            _sut.StartSyncing();
            _sut.StartSyncing();

            await _apiService.Received(1).PostWaypointsAsync(_wayPoints);
        }
        
        [Fact]
        public async Task DeletesLocalWayPointWhenItSynced()
        {
            _connectivityService.IsConnected().Returns(Task.FromResult(true));
            _dataService.GetAllWayPointsAsync().Returns(Task.FromResult(_wayPoints));
            _apiService.PostWaypointsAsync(_wayPoints)
                .Returns(Task.FromResult(
                new SaveWayPointsResponse(){SyncedWayPointsIds = _wayPoints.Select(f=>f.Id).ToList()}));
            
            _sut.StartSyncing();

            await _dataService.Received().DeleteWayPointAsync(_wayPoints.First().Id);
        }
    }
}