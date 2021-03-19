using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using Sanet.SmartSkating.Dto;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Dto.Models.Responses;
using Sanet.SmartSkating.Dto.Services;
using Sanet.SmartSkating.Services.Account;
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
        private readonly IAccountService _accountService;

        private readonly List<WayPointDto> _wayPoints = new List<WayPointDto>
        {
            new WayPointDto
            {
                Coordinate = new CoordinateDto {Latitude = 23, Longitude = 34},
                Id = "0",
                SessionId = "0",
                Time = DateTime.Now
            }
        };
        private readonly List<BleScanResultDto> _bleScans = new List<BleScanResultDto>
        {
            new BleScanResultDto()
            {
                Id = "0",
                SessionId = "0",
                Time = DateTime.Now,
                DeviceAddress = "w"
            }
        };

        private List<SessionDto> GetSessionsStub(bool saved, bool completed)
        {
            return new List<SessionDto>
            {
                new SessionDto
                {
                    Id = "0",
                    AccountId = "0",
                    IsCompleted = completed,
                    IsSaved = saved
                }
            };
        }
        
        private readonly DeviceDto _deviceStub = new DeviceDto
        {
            Id = "1",
            Manufacturer = "some",
            Model = "device",
            OsName = "os",
            OsVersion = "1.0"
        };
        
        public DataSyncServiceTests()
        {
            _dataService = Substitute.For<IDataService>();
            _apiService = Substitute.For<IApiService>();
            _connectivityService = Substitute.For<IConnectivityService>();
            _accountService = Substitute.For<IAccountService>();
            
            _sut = new DataSyncService(_dataService,_apiService,_connectivityService, _accountService);
            
            _dataService.GetAllSessionsAsync().Returns(Task.FromResult(new List<SessionDto>()));
        }

        [Fact]
        public async Task WhenStartedAndHasConnectionReadsLocalWayPointsAndSendThemToApiIfFound()
        {
            _connectivityService.IsConnected().Returns(Task.FromResult(true));
            _dataService.GetAllWayPointsAsync().Returns(Task.FromResult(_wayPoints));
            
            _sut.StartSyncing();

            await _apiService.Received().PostWaypointsAsync(_wayPoints, Arg.Any<string>());
        }
        
        [Fact]
        public async Task WhenStartedButHasNoConnection_DoesNotSendScansToApi()
        {
            _connectivityService.IsConnected().Returns(Task.FromResult(true));
            _dataService.GetAllWayPointsAsync().Returns(Task.FromResult(_wayPoints));
            
            _sut.StartSyncing();

            await _apiService.Received().PostWaypointsAsync(_wayPoints, Arg.Any<string>());
        }
        
        [Fact]
        public async Task DoesNotStartSyncServiceSecondTime()
        {
            _connectivityService.IsConnected().Returns(Task.FromResult(true));
            _dataService.GetAllWayPointsAsync().Returns(Task.FromResult(_wayPoints));
            
            _sut.StartSyncing();
            _sut.StartSyncing();

            await _apiService.Received(1).PostWaypointsAsync(_wayPoints, Arg.Any<string>());
        }
        
        [Fact]
        public async Task DeletesLocalWayPointWhenItSynced()
        {
            _connectivityService.IsConnected().Returns(Task.FromResult(true));
            _dataService.GetAllWayPointsAsync().Returns(Task.FromResult(_wayPoints));
            _apiService.PostWaypointsAsync(_wayPoints, Arg.Any<string>())
                .Returns(Task.FromResult(
                new SaveEntitiesResponse(){SyncedIds = _wayPoints.Select(f=>f.Id).ToList()}));
            
            _sut.StartSyncing();

            await _dataService.Received().DeleteWayPointAsync(_wayPoints.First().Id);
        }
        
        [Fact]
        public async Task WhenStartedAndHasConnectionReadsLocalSessionsAndSendsNotSavedToApi()
        {
            var notSavedSessions = GetSessionsStub(false, false);
            _connectivityService.IsConnected().Returns(Task.FromResult(true));
            _dataService.GetAllSessionsAsync().Returns(Task.FromResult(notSavedSessions));
            
            _sut.StartSyncing();

            await _apiService.Received().PostSessionsAsync(Arg.Any<List<SessionDto>>(), Arg.Any<string>());
        }
        
        [Fact]
        public async Task DoesNotSendsSavedSessionToApi_WhenStartedButHasNoConnection()
        {
            var notSavedSessions = GetSessionsStub(false, false);
            _connectivityService.IsConnected().Returns(Task.FromResult(false));
            _dataService.GetAllSessionsAsync().Returns(Task.FromResult(notSavedSessions));
            
            _sut.StartSyncing();

            await _apiService.DidNotReceive().PostSessionsAsync(Arg.Any<List<SessionDto>>(), Arg.Any<string>());
        }
        
        [Fact]
        public async Task WhenStartedAndHasConnectionReadsLocalSessionsButDoesNotSendSavedToApi()
        {
            var savedSessions = GetSessionsStub(true, false);
            _connectivityService.IsConnected().Returns(Task.FromResult(true));
            _dataService.GetAllSessionsAsync().Returns(Task.FromResult(savedSessions));
            
            _sut.StartSyncing();

            await _apiService.DidNotReceive().PostSessionsAsync(savedSessions, Arg.Any<string>());
        }
        
        [Fact]
        public async Task WhenStartedAndHasConnectionReadsLocalSessionsAndSendsSavedAndCompletedToApi()
        {
            var savedAndCompletedSessions = GetSessionsStub(true, true);
            _connectivityService.IsConnected().Returns(Task.FromResult(true));
            _dataService.GetAllSessionsAsync().Returns(Task.FromResult(savedAndCompletedSessions));
            
            _sut.StartSyncing();

            await _apiService.Received().PostSessionsAsync(Arg.Any<List<SessionDto>>(), Arg.Any<string>());
        }
        
        [Fact]
        public async Task MarksIncompleteSessionsAsSavedAfterSync()
        {
            var notSavedSessions = GetSessionsStub(false, false);
            _connectivityService.IsConnected().Returns(Task.FromResult(true));
            _dataService.GetAllSessionsAsync().Returns(Task.FromResult(notSavedSessions));
            _apiService.PostSessionsAsync(Arg.Any<List<SessionDto>>(), Arg.Any<string>())
                .Returns(Task.FromResult(
                    new SaveEntitiesResponse()
                    {
                        SyncedIds = notSavedSessions.Select(s=>s.Id).ToList()
                    }));
            
            _sut.StartSyncing();

            var sessionToUpdate = notSavedSessions.First();
            sessionToUpdate.IsSaved = true;
            await _dataService.Received().SaveSessionAsync(sessionToUpdate);
        }
        
        [Fact]
        public async Task DeletesCompleteSessionsAfterSync()
        {
            var completeSessions = GetSessionsStub(false, true);
            _connectivityService.IsConnected().Returns(Task.FromResult(true));
            _dataService.GetAllSessionsAsync().Returns(Task.FromResult(completeSessions));
            _apiService.PostSessionsAsync(Arg.Any<List<SessionDto>>(), Arg.Any<string>())
                .Returns(Task.FromResult(
                    new SaveEntitiesResponse()
                    {
                        SyncedIds = completeSessions.Select(s=>s.Id).ToList()
                    }));
            
            _sut.StartSyncing();
            
            await _dataService.Received().DeleteSessionAsync(completeSessions.First().Id);
        }

        #region BleScans sync
        [Fact]
        public async Task ReadsLocalScansAndSendThemToApiIfFound_WhenStarted_AndHasConnection()
        {
            _connectivityService.IsConnected().Returns(Task.FromResult(true));
            _dataService.GetAllBleScansAsync().Returns(Task.FromResult(_bleScans));
            _dataService.GetAllWayPointsAsync().Returns(Task.FromResult(new List<WayPointDto>()));
            
            _sut.StartSyncing();

            await _apiService.Received().PostBleScansAsync(_bleScans, Arg.Any<string>());
        }
        
        [Fact]
        public async Task DoesNotSendScansToApi_WhenStarted_ButHasNoConnection()
        {
            _connectivityService.IsConnected().Returns(Task.FromResult(false));
            _dataService.GetAllBleScansAsync().Returns(Task.FromResult(_bleScans));
            
            _sut.StartSyncing();

            await _apiService.DidNotReceive().PostBleScansAsync(_bleScans, Arg.Any<string>());
        }
        
        [Fact]
        public async Task DeletesLocalBleScanWhenItSynced()
        {
            _connectivityService.IsConnected().Returns(Task.FromResult(true));
            _dataService.GetAllBleScansAsync().Returns(Task.FromResult(_bleScans));
            _dataService.GetAllWayPointsAsync().Returns(Task.FromResult(new List<WayPointDto>()));
            _apiService.PostBleScansAsync(_bleScans, Arg.Any<string>())
                .Returns(Task.FromResult(
                    new SaveEntitiesResponse(){SyncedIds = _bleScans.Select(f=>f.Id).ToList()}));
            
            _sut.StartSyncing();

            await _dataService.Received().DeleteBleScanAsync(_bleScans.First().Id);
        }
        #endregion
        
        [Fact]
        public async Task GetsDeviceAndSendsItToApi_WhenConnected()
        {
            _accountService.GetDeviceInfo().Returns(_deviceStub);
            _connectivityService.IsConnected().Returns(Task.FromResult(true));
            
            _sut.StartSyncing();

            await _apiService.Received().PostDeviceAsync(_deviceStub, Arg.Any<string>());
        }
        
        [Fact]
        public async Task DoesNotSendDeviceToApi_WhenConnectionIsNotAvailable()
        {
            _accountService.GetDeviceInfo().Returns(_deviceStub);
            _connectivityService.IsConnected().Returns(Task.FromResult(false));
            
            _sut.StartSyncing();

            await _apiService.DidNotReceive().PostDeviceAsync(_deviceStub, Arg.Any<string>());
        }

        [Fact]
        public async Task SaveAndSyncSession_Saves_Session_If_No_Connection()
        {
            _connectivityService.IsConnected().Returns(Task.FromResult(false));
            var session = GetSessionsStub(false, true).First();
            
            await _sut.SaveAndSyncSessionAsync(session);

            await _dataService.Received(1).SaveSessionAsync(session);
        }
        
        [Fact]
        public async Task SaveAndSyncSession_Sends_Session_To_Api_If_HasConnection()
        {
            _connectivityService.IsConnected().Returns(Task.FromResult(true));
            var session = GetSessionsStub(false, true).First();
            _apiService.PostSessionsAsync(
                    Arg.Any<List<SessionDto>>(),
                    ApiNames.AzureApiSubscriptionKey)
                .Returns(Task.FromResult(new SaveEntitiesResponse
                {
                    SyncedIds = null
                }));
            
            await _sut.SaveAndSyncSessionAsync(session);

            await _apiService.Received(1).PostSessionsAsync(
                Arg.Any<List<SessionDto>>(),
                ApiNames.AzureApiSubscriptionKey);
        }
        
        [Fact]
        public async Task SaveAndSyncSession_Saves_Session_If_Failed_To_Send_OnServer()
        {
            _connectivityService.IsConnected().Returns(Task.FromResult(true));
            var session = GetSessionsStub(false, false).First();
            _apiService.PostSessionsAsync(
                            Arg.Any<List<SessionDto>>(),
                            ApiNames.AzureApiSubscriptionKey)
                .Returns(Task.FromResult(new SaveEntitiesResponse
            {
                SyncedIds = null
            }));
            
            await _sut.SaveAndSyncSessionAsync(session);
            
            await _dataService.Received(1).SaveSessionAsync(session);
        }
        
        [Fact]
        public async Task SaveAndSyncWaypoint_Saves_Waypoint_If_No_Connection()
        {
            _connectivityService.IsConnected().Returns(Task.FromResult(false));
            var wayPointDto = _wayPoints.First();
            
            await _sut.SaveAndSyncWayPointAsync(wayPointDto);

            await _dataService.Received(1).SaveWayPointAsync(wayPointDto);
        }
        
        [Fact]
        public async Task SaveAndSyncWaypoint_Sends_Waypoint_To_Api_If_HasConnection()
        {
            _connectivityService.IsConnected().Returns(Task.FromResult(true));
            var wayPointDto = _wayPoints.First();
            _apiService.PostWaypointsAsync(
                    Arg.Any<List<WayPointDto>>(),
                    ApiNames.AzureApiSubscriptionKey)
                .Returns(Task.FromResult(new SaveEntitiesResponse
                {
                    SyncedIds = null
                }));
            
            await _sut.SaveAndSyncWayPointAsync(wayPointDto);

            await _apiService.Received(1).PostWaypointsAsync(
                Arg.Any<List<WayPointDto>>(),
                ApiNames.AzureApiSubscriptionKey);
        }
        
        [Fact]
        public async Task SaveAndSyncWaypoint_Saves_Waypoint_If_Failed_To_Send_OnServer()
        {
            _connectivityService.IsConnected().Returns(Task.FromResult(true));
            var wayPointDto = _wayPoints.First();
            _apiService.PostWaypointsAsync(
                    Arg.Any<List<WayPointDto>>(),
                    ApiNames.AzureApiSubscriptionKey)
                .Returns(Task.FromResult(new SaveEntitiesResponse
                {
                    SyncedIds = null
                }));
            
            await _sut.SaveAndSyncWayPointAsync(wayPointDto);
            
            await _dataService.Received(1).SaveWayPointAsync(wayPointDto);
        }
    }
}