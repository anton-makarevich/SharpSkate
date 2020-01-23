using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NSubstitute;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Dto.Services;
using Sanet.SmartSkating.Models.EventArgs;
using Sanet.SmartSkating.Models.Geometry;
using Sanet.SmartSkating.Models.Location;
using Sanet.SmartSkating.Models.Training;
using Sanet.SmartSkating.Services.Account;
using Sanet.SmartSkating.Services.Api;
using Sanet.SmartSkating.Services.Location;
using Sanet.SmartSkating.Services.Tracking;
using Sanet.SmartSkating.Tests.Models.Geometry;
using Sanet.SmartSkating.ViewModels;
using Xunit;

namespace Sanet.SmartSkating.Tests.ViewModels
{
    public class LiveSessionViewModelTests
    {
        private const string NoValue = "- - -";
        
        private readonly LiveSessionViewModel _sut;
        private readonly IAccountService _accountService = Substitute.For<IAccountService>();
        private readonly ILocationService _locationService = Substitute.For<ILocationService>();
        private readonly IDataService _storageService = Substitute.For<IDataService>();
        private readonly ITrackService _trackService = Substitute.For<ITrackService>();
        private readonly ISessionService _sessionService = Substitute.For<ISessionService>();
        private readonly Coordinate _locationStub = new Coordinate(23, 45);
        private readonly IDataSyncService _dataSyncService = Substitute.For<IDataSyncService>();
        private readonly IBleLocationService _bleLocationService = Substitute.For<IBleLocationService>();

        public LiveSessionViewModelTests()
        {
            _sut = new LiveSessionViewModel(
                _locationService,
                _storageService,
                _trackService,
                _sessionService,
                _accountService,
                _dataSyncService,
                _bleLocationService);
        }

        [Fact]
        public void StartsLocationServiceWhenStartButtonPressed()
        {
            _sut.StartCommand.Execute(null);

            _locationService.Received().StartFetchLocation();
        }
        
        [Fact]
        public void StartSavesSessionToLocalStorage()
        {
            var session = CreateSessionMock();
            const string userId = "123";
            _accountService.UserId.Returns(userId);

            _sut.StartCommand.Execute(null);

            _storageService.Received().SaveSessionAsync(Arg.Is<SessionDto>(s=>
                s.Id == session.SessionId 
                && s.AccountId == userId));
        }
        
        [Fact]
        public async Task FetchesKnownBleDevices_WhenPageIsLoaded()
        {
            _sut.AttachHandlers();

            await _bleLocationService.Received().LoadDevicesDataAsync();
        }
        
        [Fact]
        public void StartsLocationServiceWhenPageIsLoaded()
        {
           _sut.AttachHandlers();

            _locationService.Received().StartFetchLocation();
        }

        [Fact]
        public void StopSavesCompletedSessionToLocalStorage()
        {
            var session = CreateSessionMock();
            const string userId = "123";
            _accountService.UserId.Returns(userId);
            
            _sut.StopCommand.Execute(null);

            _storageService.Received().SaveSessionAsync(Arg.Is<SessionDto>(s=>
                s.Id == session.SessionId 
                && s.AccountId == userId
                && s.IsCompleted));
        }
        
        [Fact]
        public async Task StopSyncsDataForSessionsAndWayPoints()
        {
            _sut.StopCommand.Execute(null);

            await _dataSyncService.Received().SyncSessionsAsync();
            await _dataSyncService.Received().SyncWayPointsAsync();
        }
        
        [Fact]
        public void StopsLocationService_WhenStopButtonPressed()
        {
            _sut.StopCommand.Execute(null);

            _locationService.Received().StopFetchLocation();
        }

        [Fact]
        public void ChangesStateToIsRunning_WhenStartButtonPressed()
        {
            var isRunningChanged = false;
            _sut.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(_sut.IsRunning)) isRunningChanged = true;
            };

            _sut.StartCommand.Execute(null);

            Assert.True(_sut.IsRunning);
            Assert.True(isRunningChanged);
        }

        [Fact]
        public void ChangesStateToNotIsRunning_WhenStartButtonPressed()
        {
            _sut.StartCommand.Execute(null);

            var isRunningChanged = false;
            _sut.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(_sut.IsRunning)) isRunningChanged = true;
            };

            _sut.StopCommand.Execute(null);

            Assert.False(_sut.IsRunning);
            Assert.True(isRunningChanged);
        }

        [Fact]
        public void StopsLocationService_WhenLeavingThePage()
        {
            _sut.StartCommand.Execute(null);

            _sut.DetachHandlers();

            _locationService.Received().StopFetchLocation();
            Assert.False(_sut.IsRunning);
        }

        [Fact]
        public void UpdatesLastLocationWithNewValueFromService_IfServiceIsStarted()
        {
            InitViewModelWithRink();
            _sut.StartCommand.Execute(null);
            _locationService.LocationReceived += Raise.EventWith(null, new CoordinateEventArgs(_locationStub));

            Assert.Equal(_locationStub, _sut.LastCoordinate);
        }

        [Fact]
        public void LastCoordinateChangeUpdatesInfoLabel()
        {
            InitViewModelWithRink();
            _sut.StartCommand.Execute(null);
            _locationService.LocationReceived += Raise.EventWith(null, new CoordinateEventArgs(_locationStub));

            Assert.Contains(_sut.InfoLabel, _locationStub.ToString());
        }

        [Fact]
        public void StopClearsInfoLabel()
        {
            InitViewModelWithRink();
            _sut.StartCommand.Execute(null);
            _locationService.LocationReceived += Raise.EventWith(null, new CoordinateEventArgs(_locationStub));

            _sut.StopCommand.Execute(true);

            Assert.Empty(_sut.InfoLabel);
        }

        [Fact]
        public async Task LastCoordinateChangeSavesCoordinateToDisk()
        {
            InitViewModelWithRink();
            _sut.StartCommand.Execute(null);
            _locationService.LocationReceived += Raise.EventWith(null, new CoordinateEventArgs(_locationStub));

            await _storageService.Received().SaveWayPointAsync(Arg.Any<WayPointDto>());
        }

        [Fact]
        public void CreatesSessionOnPageStart()
        {
            var rink = InitViewModelWithRink();

            _sessionService.Received().CreateSessionForRink(rink);
        }

        private Rink InitViewModelWithRink()
        {
            var rink = new Rink(RinkTests.EindhovenStart,
                RinkTests.EindhovenFinish);
            _trackService.SelectedRink.Returns(rink);

            _sut.AttachHandlers();
            return rink;
        }

        [Fact]
        public void CannotStartIfSessionIsNull()
        {
            _sut.AttachHandlers();
            
            Assert.False(_sut.CanStart);
        }
        
        [Fact]
        public void CanStartIfSessionIsCreated()
        {
            _trackService.SelectedRink.Returns(
                new Rink(RinkTests.EindhovenStart,
                    RinkTests.EindhovenFinish));
            _sut.AttachHandlers();
            
            Assert.True(_sut.CanStart);
        }
        
        [Fact]
        public void ChangesCanStartWhenSessionIsCreated()
        {
            _trackService.SelectedRink.Returns(
                new Rink(RinkTests.EindhovenStart,
                    RinkTests.EindhovenFinish));

            var canStartChanged = false;
            _sut.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(_sut.CanStart)) canStartChanged = true;
            };

            _sut.AttachHandlers();
            
            Assert.True(canStartChanged);
        }

        [Fact]
        public void SessionIsUpdatedWhenLocationIsReceived()
        {
            var session = CreateSessionMock();

            _sut.StartCommand.Execute(null);
            _locationService.LocationReceived += Raise.EventWith(null, new CoordinateEventArgs(_locationStub));
            
            session.Received().AddPoint(_locationStub, Arg.Any<DateTime>());
        }

        private ISession CreateSessionMock()
        {
            var rink = new Rink(RinkTests.EindhovenStart,
                RinkTests.EindhovenFinish);
            _trackService.SelectedRink.Returns(rink);
            var session = Substitute.For<ISession>();
            _sessionService.CreateSessionForRink(rink).Returns(session);
            _sut.AttachHandlers();
            return session;
        }

        [Fact]
        public void ShowsLastLapTime()
        {
            var session = CreateSessionMock();
            session.LastLapTime.Returns(new TimeSpan(0, 0, 40));
            session.LapsCount.Returns(1);
            
            _sut.StartCommand.Execute(null);
            _locationService.LocationReceived += Raise.EventWith(null, new CoordinateEventArgs(_locationStub));

            Assert.Equal("0:00:40",_sut.LastLapTime);
        }
        
        [Fact]
        public void ShowsPlaceholderForLastLapTimeIfNoLapsDone()
        {
            var session = CreateSessionMock();
            session.LapsCount.Returns(0);
            
            _sut.StartCommand.Execute(null);
            _locationService.LocationReceived += Raise.EventWith(null, new CoordinateEventArgs(_locationStub));

            Assert.Equal(NoValue,_sut.LastLapTime);
        }
        
        [Fact]
        public void ShowsBestLapTime()
        {
            var session = CreateSessionMock();
            session.BestLapTime.Returns(new TimeSpan(0, 0, 40));
            session.LapsCount.Returns(1);
            
            _sut.StartCommand.Execute(null);
            _locationService.LocationReceived += Raise.EventWith(null, new CoordinateEventArgs(_locationStub));

            Assert.Equal("0:00:40",_sut.BestLapTime);
        }
        
        [Fact]
        public void ShowsPlaceholderForBestLapTimeIfNoLapsDone()
        {
            var session = CreateSessionMock();
            session.LapsCount.Returns(0);
            
            _sut.StartCommand.Execute(null);
            _locationService.LocationReceived += Raise.EventWith(null, new CoordinateEventArgs(_locationStub));

            Assert.Equal(NoValue,_sut.BestLapTime);
        }
        
        [Fact]
        public void ShowsAmountOfLaps()
        {
            var session = CreateSessionMock();
            session.LapsCount.Returns(1);
            
            _sut.StartCommand.Execute(null);
            _locationService.LocationReceived += Raise.EventWith(null, new CoordinateEventArgs(_locationStub));

            Assert.Equal("1",_sut.Laps);
        }
        
        [Fact]
        public void ShowsZeroForAmountOfLapsIfNoLapsDone()
        {
            var session = CreateSessionMock();
            session.LapsCount.Returns(0);
            
            _sut.StartCommand.Execute(null);
            _locationService.LocationReceived += Raise.EventWith(null, new CoordinateEventArgs(_locationStub));

            Assert.Equal("0",_sut.Laps);
        }
        
        [Fact]
        public void ShowsLastSectorTime()
        {
            CreateSessionMockWithOneSector();

            _sut.StartCommand.Execute(null);
            _locationService.LocationReceived += Raise.EventWith(null, new CoordinateEventArgs(_locationStub));

            Assert.Equal("00:10",_sut.LastSectorTime);
        }
        
        [Fact]
        public void ShowsPlaceholderForLastSectorTimeIfNoSectorsDone()
        {
            var session = CreateSessionMock();
            session.Sectors.Returns(new List<Section>());

            _sut.StartCommand.Execute(null);
            _locationService.LocationReceived += Raise.EventWith(null, new CoordinateEventArgs(_locationStub));

            Assert.Equal(NoValue,_sut.LastSectorTime);
        }
        
        [Fact]
        public void ShowsBestSectorTime()
        {
            CreateSessionMockWithOneSector();

            _sut.StartCommand.Execute(null);
            _locationService.LocationReceived += Raise.EventWith(null, new CoordinateEventArgs(_locationStub));

            Assert.Equal("00:10",_sut.BestSectorTime);
        }
        
        [Fact]
        public void ShowsPlaceholderForBestSectorTimeIfNoSectorsDone()
        {
            var session = CreateSessionMock();
            session.Sectors.Returns(new List<Section>());

            _sut.StartCommand.Execute(null);
            _locationService.LocationReceived += Raise.EventWith(null, new CoordinateEventArgs(_locationStub));

            Assert.Equal(NoValue,_sut.BestSectorTime);
        }

        private void CreateSessionMockWithOneSector()
        {
            var session = CreateSessionMock();
            var startTime = DateTime.Now;
            var endTime = startTime.AddSeconds(10);
            var section = new Section(
                new WayPoint(
                    _locationStub,
                    _locationStub,
                    startTime, 
                    WayPointTypes.Start),
                new WayPoint(
                    _locationStub, 
                    _locationStub,
                    endTime, 
                    WayPointTypes.Finish)
            );
            session.Sectors.Returns(new List<Section>() {section});
            session.BestSector.Returns(section);
        }

        [Fact]
        public void DisplaysTotalDistance()
        {
            CreateSessionMockWithOneSector();

            _sut.StartCommand.Execute(null);
            _locationService.LocationReceived += Raise.EventWith(null, new CoordinateEventArgs(_locationStub));

            Assert.Equal("0.1Km",_sut.Distance);
        }

        [Fact]
        public void StartingSessionUpdatesItsStartTime()
        {
            var session = CreateSessionMock();
            var startTime = DateTime.Now;
            var endTime = startTime.AddSeconds(10);
            var section = new Section(
                new WayPoint(_locationStub,_locationStub,startTime, WayPointTypes.Start),
                new WayPoint(_locationStub,_locationStub,endTime, WayPointTypes.Finish)
            );
            session.Sectors.Returns(new List<Section>(){section});
            
            _sut.StartCommand.Execute(null);
            
            session.Received().SetStartTime(Arg.Any<DateTime>());
        }

        #region Ble
        [Fact]
        public void StartsBleScan_WhenStartButtonPressed()
        {
            _sut.StartCommand.Execute(null);

            _bleLocationService.Received().StartBleScan(Arg.Any<string>());
        }
        
        [Fact]
        public void StopsBleScan_WhenStopButtonPressed()
        {
            _sut.StopCommand.Execute(null);

            _bleLocationService.Received().StopBleScan();
        }
        
        [Fact]
        public void AddsSectionSeparator_WhenCheckPointIsPassed()
        {
            var session = CreateSessionMock();
            _sut.StartCommand.Execute(null);
            const WayPointTypes checkPointType = WayPointTypes.Start;
            var date = DateTime.Now;
            
            _bleLocationService.CheckPointPassed += Raise.EventWith(
                null, 
                new CheckPointEventArgs(checkPointType, date));

            session.Received().AddSeparatingPoint(checkPointType,date);
        }
        #endregion
    }
}