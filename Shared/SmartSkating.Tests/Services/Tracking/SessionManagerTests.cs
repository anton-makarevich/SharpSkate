using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Sanet.SmartSkating.Dto;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Dto.Models.Responses;
using Sanet.SmartSkating.Dto.Services;
using Sanet.SmartSkating.Models.EventArgs;
using Sanet.SmartSkating.Models.Geometry;
using Sanet.SmartSkating.Models.Location;
using Sanet.SmartSkating.Models.Training;
using Sanet.SmartSkating.Services;
using Sanet.SmartSkating.Services.Account;
using Sanet.SmartSkating.Services.Api;
using Sanet.SmartSkating.Services.Location;
using Sanet.SmartSkating.Services.Tracking;
using Sanet.SmartSkating.Tests.Models.Geometry;
using Xunit;

namespace Sanet.SmartSkating.Tests.Services.Tracking
{
    public class SessionManagerTests
    {
        private readonly Coordinate _locationStub = new Coordinate(23, 45);
        private readonly Rink _rink = new Rink(RinkTests.EindhovenStart,RinkTests.EindhovenFinish,"rinkId");

        private readonly ISettingsService _settingsService = Substitute.For<ISettingsService>();
        private readonly SessionManager _sut;
        private readonly ILocationService _locationService = Substitute.For<ILocationService>();
        private readonly IDataService _dataService = Substitute.For<IDataService>();
        private readonly IAccountService _accountService = Substitute.For<IAccountService>();
        private readonly IDataSyncService _dataSyncService = Substitute.For<IDataSyncService>();
        private readonly IBleLocationService _bleLocationService = Substitute.For<IBleLocationService>();
        private readonly ISessionProvider _sessionProvider = Substitute.For<ISessionProvider>();
        private readonly IApiService _apiClient = Substitute.For<IApiService>();
        private readonly ISyncService _syncService = Substitute.For<ISyncService>();

        public SessionManagerTests()
        {
            _sut = new SessionManager(_locationService,
                _dataService,
                _accountService,
                _dataSyncService,
                _bleLocationService,
                _settingsService,
                _sessionProvider,
                _apiClient,
                _syncService
                );
        }

        [Fact]
        public void SessionManager_Is_Not_Ready_When_SessionProvider_DoesNot_Have_Current_Session()
        {
            _sessionProvider.CurrentSession.ReturnsNull();

            _sut.IsReady.Should().BeFalse();
        }

        [Fact]
        public void SessionManager_Is_Ready_When_SessionProvider_Has_Current_Session()
        {
            _sessionProvider.CurrentSession.Returns(Substitute.For<ISession>());

            _sut.IsReady.Should().BeTrue();
        }

        [Fact]
        public async Task  StartSession_Starts_LocationService_When_IsReady()
        {
            _sessionProvider.CurrentSession.Returns(Substitute.For<ISession>());

            await _sut.StartSession();

            _locationService.Received().StartFetchLocation();
        }

        [Fact]
        public async Task  StartSession_DoesNot_Start_LocationService_When_IsNotReady()
        {
            _sessionProvider.CurrentSession.ReturnsNull();

            await _sut.StartSession();

            _locationService.DidNotReceive().StartFetchLocation();
        }
        
        [Fact]
        public async Task  StartSession_DoesNot_Start_LocationService_When_Session_IsRemote()
        {
            var session = PrepareSessionMock("", "", "");
            session.IsRemote.Returns(true);

            await _sut.StartSession();

            _locationService.DidNotReceive().StartFetchLocation();
        }

        [Fact]
        public async Task  StartSession_Turns_IsRunning_When_IsReady()
        {
            _sessionProvider.CurrentSession.Returns(Substitute.For<ISession>());

            await _sut.StartSession();

            _sut.IsRunning.Should().BeTrue();
        }

        [Fact]
        public async Task  StartSession_DoesNot_Turn_IsRunning_When_IsNotReady()
        {
            _sessionProvider.CurrentSession.ReturnsNull();

            await _sut.StartSession();

            _sut.IsRunning.Should().BeFalse();
        }

        [Fact]
        public async Task  StopSession_Turns_Off_IsRunning()
        {
            _sessionProvider.CurrentSession.Returns(Substitute.For<ISession>());

            await _sut.StartSession();
            _sut.IsRunning.Should().BeTrue();
            _sut.StopSession();
            _sut.IsRunning.Should().BeFalse();
        }

        [Fact]
        public async Task Start_Saves_Session_To_Local_Storage()
        {
            const string sessionId = "sessionId";
            const string userId = "123";
            const string deviceId = "deviceId";

            PrepareSessionMock(sessionId, userId, deviceId);

            await _sut.StartSession();

            await _dataService.Received().SaveSessionAsync(Arg.Is<SessionDto>(s=>
                s.Id == sessionId
                && s.AccountId == userId
                && s.RinkId == _rink.Id
                && s.DeviceId == deviceId
            ));
        }

        [Fact]
        public async Task Stop_Saves_CompletedSessionToLocalStorage()
        {
            const string sessionId = "someSessionId";
            const string userId = "123";
            const string deviceId = "deviceId";
            PrepareSessionMock(sessionId, userId, deviceId);

            _sut.StopSession();

            await _dataService.Received().SaveSessionAsync(Arg.Is<SessionDto>(s =>
                s.Id == sessionId
                && s.AccountId == userId
                && s.IsCompleted
                && s.RinkId == _rink.Id
                && s.DeviceId == deviceId
            ));
        }

        [Fact]
        public async Task StartSession_Fetches_KnownBle_Devices_When_Ble_Is_On_In_Settings()
        {
            _settingsService.UseBle.Returns(true);

            await _sut.StartSession();

            await _bleLocationService.Received().LoadDevicesDataAsync();
        }

        [Fact]
        public async Task StartSession_DoesNot_Fetch_KnownBle_Devices_When_Ble_Is_Off_In_Settings()
        {
            _settingsService.UseBle.Returns(false);

            await _sut.StartSession();

            await _bleLocationService.DidNotReceive().LoadDevicesDataAsync();
        }

        [Fact]
        public async Task StopSession_SyncsData_For_Sessions_And_WayPoints()
        {
            const string sessionId = "someSessionId";
            const string userId = "123";
            const string deviceId = "deviceId";
            PrepareSessionMock(sessionId, userId, deviceId);

            _sut.StopSession();

            await _dataSyncService.Received().SyncSessionsAsync();
            await _dataSyncService.Received().SyncWayPointsAsync();
        }

        [Fact]
        public void StopSession_Stops_LocationService()
        {
            _sut.StopSession();

            _locationService.Received().StopFetchLocation();
        }

        [Fact]
        public async Task LastCoordinateChangeSavesCoordinateToDisk()
        {
            await _sut.StartSession();
            _locationService.LocationReceived += Raise.EventWith(null, new CoordinateEventArgs(_locationStub));

            await _dataService.Received().SaveWayPointAsync(Arg.Any<WayPointDto>());
        }

        [Fact]
        public async Task Session_Is_Updated_Whe_nLocation_Is_Received()
        {
            await _sut.StartSession();
            _locationService.LocationReceived += Raise.EventWith(null, new CoordinateEventArgs(_locationStub));

            _sut.CurrentSession?.Received()?.AddPoint(_locationStub, Arg.Any<DateTime>());
        }

        [Fact]
        public async Task StartsBleScan_WhenStartButtonPressed_AndBleAllowedInSettings()
        {
            _settingsService.UseBle.Returns(true);

            await _sut.StartSession();

            _bleLocationService.Received().StartBleScan(Arg.Any<string>());
        }

        [Fact]
        public async Task DoesNotStartBleScan_WhenStartButtonPressed_ButBleIsNotAllowedInSettings()
        {
            _settingsService.UseBle.Returns(false);

            await _sut.StartSession();

            _bleLocationService.DidNotReceive().StartBleScan(Arg.Any<string>());
        }

        [Fact]
        public void StopsBleScan_WhenStopButtonPressed()
        {
            _sut.StopSession();

            _bleLocationService.Received().StopBleScan();
        }

        [Fact]
        public async Task AddsSectionSeparator_WhenCheckPointIsPassed()
        {
            _settingsService.UseBle.Returns(true);
            var session = Substitute.For<ISession>();
            _sessionProvider.CurrentSession.Returns(session);
            await _sut.StartSession();
            const WayPointTypes checkPointType = WayPointTypes.Start;
            var date = DateTime.Now;

            _bleLocationService.CheckPointPassed += Raise.EventWith(
                null,
                new CheckPointEventArgs(checkPointType, date));

            session.Received().AddSeparatingPoint(checkPointType,date);
        }

        [Fact]
        public async Task StartSession_Changes_State_To_IsRunning()
        {
            await _sut.StartSession();

            Assert.True(_sut.IsRunning);
        }

        [Fact]
        public async Task StopSession_ChangesStateToNotIsRunning()
        {
            await _sut.StartSession();

            _sut.StopSession();

            Assert.False(_sut.IsRunning);
        }

        [Fact]
        public void CheckSession_Checks_If_ActiveSession_Is_Remote()
        {
            var session = PrepareSessionMock("SessionId", "userId", "deviceId");

            _sut.CheckSession();

            var _ = session.Received().IsRemote;
        }

        [Fact]
        public void IsRunning_Is_True_When_Session_Is_Remote()
        {
            var session = PrepareSessionMock("SessionId", "userId", "deviceId");
            session.IsRemote.Returns(true);
            
            _sut.CheckSession();

            _sut.IsRunning.Should().BeTrue();
        }

        [Fact]
        public void StopSession_Does_Nothing_When_Session_Is_Remote()
        {
            var session = PrepareSessionMock("SessionId", "userId", "deviceId");
            session.IsRemote.Returns(true);
            _sut.CheckSession();
            
            _sut.StopSession();

            _sut.IsRunning.Should().BeTrue();
            _bleLocationService.DidNotReceive().StopBleScan();
            _locationService.DidNotReceive().StopFetchLocation();
        }

        [Fact]
        public void CheckSession_Starts_SyncServer_Connection_For_Remote_Session()
        {
            const string sessionId = "sessionId";
            var session = PrepareSessionMock(sessionId, "userId", "deviceId");
            session.IsRemote.Returns(true);
            
            _sut.CheckSession();

            _syncService.Received(1).ConnectToHub(sessionId);
        }
        
        [Fact]
        public void CheckSession_Gets_Waypoints_For_Remote_Session()
        {
            const string sessionId = "sessionId";
            var session = PrepareSessionMock(sessionId, "userId", "deviceId");
            session.IsRemote.Returns(true);
            
            _sut.CheckSession();

            _apiClient.Received(1)
                .GetWaypointsForSessionAsync(sessionId, ApiNames.AzureApiSubscriptionKey);
        }

        [Fact]
        public void CheckSession_Updates_Session_WithWaypoints_From_Api()
        {
            const string sessionId = "sessionId";
            var session = PrepareSessionMock(sessionId, "userId", "deviceId");
            session.IsRemote.Returns(true);
            var time = DateTime.Now;
            var coordinateDto = new CoordinateDto
            {
                Latitude = 123,
                Longitude = 456
            };
            var coordinate = new Coordinate(coordinateDto);
            _apiClient.GetWaypointsForSessionAsync(sessionId, ApiNames.AzureApiSubscriptionKey)
                .Returns(new GetWaypointsResponse
                {
                    Waypoints = new List<WayPointDto>
                    {
                        new WayPointDto
                        {
                            SessionId = sessionId,
                            Coordinate = coordinateDto,
                            Time = time
                        }
                    }
                });
            
            _sut.CheckSession();
            
            session.Received().AddPoint(coordinate, time);
        }
        
        [Fact]
        public void Updates_Session_WithWaypoint_From_SyncHub()
        {
            const string sessionId = "sessionId";
            var session = PrepareSessionMock(sessionId, "userId", "deviceId");
            session.IsRemote.Returns(true);
            var time = DateTime.Now;
            var coordinateDto = new CoordinateDto
            {
                Latitude = 123,
                Longitude = 456
            };
            var coordinate = new Coordinate(coordinateDto);
            var waypoint = new WayPointDto
            {
                SessionId = sessionId,
                Coordinate = coordinateDto,
                Time = time
            };

            _sut.CheckSession();
            _syncService.WayPointReceived += Raise.EventWith(null, new WayPointEventArgs(waypoint));
            
            session.Received().AddPoint(coordinate, time);
        }

        private ISession PrepareSessionMock(string sessionId, string userId, string deviceId)
        {
            var session = Substitute.For<ISession>();
            session.Rink.Returns(_rink);
            session.SessionId.Returns(sessionId);
            _sessionProvider.CurrentSession.Returns(session);
            _accountService.UserId.Returns(userId);
            _accountService.DeviceId.Returns(deviceId);
            return session;
        }
     }
}
