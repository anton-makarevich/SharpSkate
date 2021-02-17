using System;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Dto.Services;
using Sanet.SmartSkating.Models.EventArgs;
using Sanet.SmartSkating.Models.Geometry;
using Sanet.SmartSkating.Models.Location;
using Sanet.SmartSkating.Services;
using Sanet.SmartSkating.Services.Account;
using Sanet.SmartSkating.Services.Api;
using Sanet.SmartSkating.Services.Location;
using Sanet.SmartSkating.Services.Tracking;
using Sanet.SmartSkating.Tests.Models.Geometry;
using Xunit;

namespace Sanet.SmartSkating.Tests.Services.Tracking
{
    public class SessionServiceTests
    {
        private readonly Coordinate _locationStub = new Coordinate(23, 45);
        private readonly Rink _rink = new Rink(RinkTests.EindhovenStart,RinkTests.EindhovenFinish,"rinkId");

        private readonly ISettingsService _settingsService = Substitute.For<ISettingsService>();
        private readonly SessionService _sut;
        private readonly ILocationService _locationService = Substitute.For<ILocationService>();
        private readonly IDataService _dataService = Substitute.For<IDataService>();
        private readonly ITrackService _trackService = Substitute.For<ITrackService>();
        private readonly IAccountService _accountService = Substitute.For<IAccountService>();
        private readonly IDataSyncService _dataSyncService = Substitute.For<IDataSyncService>();
        private readonly IBleLocationService _bleLocationService = Substitute.For<IBleLocationService>();

        public SessionServiceTests()
        {
            _sut = new SessionService(_locationService,
                _dataService,
                _trackService,
                _accountService,
                _dataSyncService,
                _bleLocationService,
                _settingsService);
        }

        [Fact]
        public async Task StartSession_Creates_New_Session_For_Rink()
        {
            _trackService.SelectedRink.Returns(_rink);

            await _sut.StartSession();

            _sut.CurrentSession?.Rink.Should().Be(_rink);
        }

        [Fact]
        public async Task CreateSession_Assigns_Value_To_CurrentSession_When_Rink_Is_Selected()
        {
            _trackService.SelectedRink.Returns(_rink);

            await _sut.StartSession();

            _sut.CurrentSession.Should().NotBeNull();
        }

        [Fact]
        public async Task  CreateSession_DoesNot_Assign_Value_To_CurrentSession_When_Rink_Is_Not_Selected()
        {
            await _sut.StartSession();

            _sut.CurrentSession.Should().BeNull();
        }

        [Fact]
        public async Task  StartsLocationServiceWhenStartButtonPressed()
        {
            await _sut.StartSession();

            _locationService.Received().StartFetchLocation();
        }

        [Fact]
        public async Task Start_Saves_Session_To_Local_Storage()
        {
            const string rinkId ="rinkId";
            const string sessionId = "sessionId";
            const string userId = "123";
            const string deviceId = "deviceId";
            var deviceInfo = new DeviceDto
            {
                Id = deviceId
            };

            _accountService.UserId.Returns(userId);
            _accountService.GetDeviceInfo().Returns(deviceInfo);

            await _sut.StartSession();

            await _dataService.Received().SaveSessionAsync(Arg.Is<SessionDto>(s=>
                s.Id == sessionId
                && s.AccountId == userId
                && s.RinkId == rinkId
                && s.DeviceId == deviceId
            ));
        }

        [Fact]
        public async Task Stop_Saves_CompletedSessionToLocalStorage()
        {
            const string rinkId ="rinkId";
            const string sessionId = "someSessionId";
            const string userId = "123";
            _accountService.UserId.Returns(userId);
            const string deviceId = "deviceId";
            var deviceInfo = new DeviceDto
            {
                Id = deviceId
            };
            _accountService.GetDeviceInfo().Returns(deviceInfo);

            _sut.StopSession();

            await _dataService.Received().SaveSessionAsync(Arg.Is<SessionDto>(s=>
                s.Id == sessionId
                && s.AccountId == userId
                && s.IsCompleted
                && s.RinkId == rinkId
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
        public async Task StartSession_Starts_LocationService()
        {
            await _sut.StartSession();

            _locationService.Received().StartFetchLocation();
        }

        [Fact]
        public async Task StopSession_SyncsData_For_Sessions_And_WayPoints()
        {
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
        public void AddsSectionSeparator_WhenCheckPointIsPassed()
        {
            _settingsService.UseBle.Returns(true);
            // var session = CreateSessionMock();
            // _sut.StartCommand.Execute(null);
            const WayPointTypes checkPointType = WayPointTypes.Start;
            var date = DateTime.Now;

            _bleLocationService.CheckPointPassed += Raise.EventWith(
                null,
                new CheckPointEventArgs(checkPointType, date));

            //session.Received().AddSeparatingPoint(checkPointType,date);
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
     }
}
