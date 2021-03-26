using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Sanet.SmartSkating.Models.EventArgs;
using Sanet.SmartSkating.Models.Geometry;
using Sanet.SmartSkating.Models.Location;
using Sanet.SmartSkating.Services;
using Sanet.SmartSkating.Services.Api;
using Sanet.SmartSkating.Services.Hardware;
using Sanet.SmartSkating.Services.Location;
using Sanet.SmartSkating.Services.Tracking;
using Sanet.SmartSkating.Tests.Models.Geometry;
using Sanet.SmartSkating.ViewModels;
using Xunit;

namespace Sanet.SmartSkating.Tests.ViewModels
{
    public class StartViewModelTest
    {
        private const string RinkId = "rinkId";

        private readonly StartViewModel _sut;
        private readonly ILocationService _locationService = Substitute.For<ILocationService>();
        private readonly Coordinate _locationStub = new Coordinate(23, 45);
        private readonly ITrackService _tracksService = Substitute.For<ITrackService>();
        private readonly INavigationService _navigationService = Substitute.For<INavigationService>();
        private readonly IDataSyncService _dataSyncService = Substitute.For<IDataSyncService>();
        private readonly IBluetoothService _bluetoothService = Substitute.For<IBluetoothService>();
        private readonly ISettingsService _settingsService = Substitute.For<ISettingsService>();
        private readonly ISessionProvider _sessionProvider = Substitute.For<ISessionProvider>();

        public StartViewModelTest()
        {
            _sut = new StartViewModel(_locationService,_tracksService, _dataSyncService,
                _bluetoothService, _settingsService, _sessionProvider);
            _sut.SetNavigationService(_navigationService);
        }

        [Fact]
        public void MakesGeoLocationRequestOnPageAppear()
        {
            _sut.AttachHandlers();

            _locationService.Received().StartFetchLocation();
        }

        [Fact]
        public void StartsDataSyncProcessOnPageAppear()
        {
            _sut.AttachHandlers();

            _dataSyncService.Received().StartSyncing();
        }

        [Fact]
        public void IsInitializingGeoServicesTrueOnLocationRequest()
        {
            _sut.AttachHandlers();

            Assert.True(_sut.IsInitializingGeoServices);
        }

        [Fact]
        public async Task LoadsTracksFromServiceOnPageAppear()
        {
            _sut.AttachHandlers();

            await _tracksService.Received().LoadTracksAsync();
        }

        [Fact]
        public void StopsGeoLocationFetchingAfterCoordinateHasBeenReceived()
        {
            _sut.AttachHandlers();
            _locationService.LocationReceived += Raise.EventWith(null, new CoordinateEventArgs(_locationStub));

            _locationService.Received().StopFetchLocation();
        }

        [Fact]
        public void IsInitializingGeoServicesIsFalseAfterCoordinateHasBeenReceived()
        {
            _sut.AttachHandlers();
            _locationService.LocationReceived += Raise.EventWith(null, new CoordinateEventArgs(_locationStub));

            _locationService.Received().StopFetchLocation();
            Assert.False(_sut.IsInitializingGeoServices);
        }

        [Fact]
        public void ShowsInformationLabelDuringGeoFetching()
        {
            _sut.AttachHandlers();

            Assert.Equal("Initializing GeoServices. Be sure you're in open air", _sut.InfoLabel);
        }

        [Fact]
        public void GeoServicesAreNotInitializedWhenCoordinateHasNotBeenReceived()
        {
            _sut.AttachHandlers();
            _locationService.LocationReceived += Raise.EventWith(null, new CoordinateEventArgs(new Coordinate()));

            _sut.AreGeoServicesInitialized.Should().BeFalse();
        }

        [Fact]
        public void GeoServicesAreInitializedWhenCoordinateHasBeenReceived()
        {
            _sut.AttachHandlers();
            _locationService.LocationReceived += Raise.EventWith(null, new CoordinateEventArgs(_locationStub));

            Assert.True(_sut.AreGeoServicesInitialized);
        }

        [Fact]
        public void TriesToSelectNearestRinkWhenCoordinateHasBeenReceived()
        {
            _sut.AttachHandlers();
            _locationService.LocationReceived += Raise.EventWith(null, new CoordinateEventArgs(_locationStub));

            _tracksService.Received().SelectRinkCloseTo(_locationStub);
        }

        [Fact]
        public void DoesNotShowAnyMessagesWhenTrackIsSelected()
        {
            _tracksService.SelectedRink.Returns(new Rink(
                RinkTests.EindhovenStart,
                RinkTests.EindhovenFinish,
                RinkId));

            _sut.AttachHandlers();
            _locationService.LocationReceived += Raise.EventWith(null, new CoordinateEventArgs(_locationStub));

            Assert.Empty(_sut.InfoLabel);
        }

        [Fact]
        public void ShowsMessageToSelectRinkManuallyWhenTrackIsNotSelected()
        {
            _sut.AttachHandlers();
            _locationService.LocationReceived += Raise.EventWith(null, new CoordinateEventArgs(_locationStub));

            Assert.Equal("No known Rinks nearby, please select manually",_sut.InfoLabel);
        }

        [Fact]
        public void SetsIsTrackSelectedToTrueWhenRinkIsSelected()
        {
            _tracksService.SelectedRink.Returns(new Rink(RinkTests.EindhovenStart, RinkTests.EindhovenFinish,RinkId));

            _sut.AttachHandlers();
            _locationService.LocationReceived += Raise.EventWith(null, new CoordinateEventArgs(_locationStub));

            Assert.True(_sut.IsRinkSelected);
        }

        [Fact]
        public void IsTrackSelectedIsUpdatedWhenCoordinateHasBeenReceived()
        {
            _tracksService.SelectedRink.Returns(new Rink(RinkTests.EindhovenStart, RinkTests.EindhovenFinish,RinkId));
            var isTrackSelectedUpdated = 0;
            _sut.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(_sut.IsRinkSelected))
                    isTrackSelectedUpdated = +1;
            };

            _sut.AttachHandlers();
            _locationService.LocationReceived += Raise.EventWith(null, new CoordinateEventArgs(_locationStub));

            Assert.Equal(1, isTrackSelectedUpdated);
        }

        [Fact]
        public void ShowsTrackNameWhenTrackIsSelected()
        {
            const string trackName = "Eindhoven";
            _tracksService.SelectedRink.Returns(new Rink(
                RinkTests.EindhovenStart,
                RinkTests.EindhovenFinish,
                "trackId",
                trackName));

            _sut.AttachHandlers();
            _locationService.LocationReceived += Raise.EventWith(null, new CoordinateEventArgs(_locationStub));

            Assert.Equal(trackName,_sut.RinkName);
        }

        [Fact]
        public void TrackNameIsUpdatedWhenTrackIsSelected()
        {
            _tracksService.SelectedRink.Returns(new Rink(RinkTests.EindhovenStart, RinkTests.EindhovenFinish,RinkId));
            var trackNameUpdated = 0;
            _sut.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(_sut.RinkName))
                    trackNameUpdated = +1;
            };

            _sut.AttachHandlers();
            _locationService.LocationReceived += Raise.EventWith(null, new CoordinateEventArgs(_locationStub));

            Assert.Equal(1, trackNameUpdated);
        }

        [Fact]
        public void CannotStartIfGpsIsInitializing()
        {
            _sut.AttachHandlers();
            Assert.False(_sut.CanStart);
        }

        [Fact]
        public void CannotStartIfGpsIsInitializedButTrackIsNotSelected()
        {
            _sut.AttachHandlers();
            _locationService.LocationReceived += Raise.EventWith(null, new CoordinateEventArgs(_locationStub));

            Assert.False(_sut.CanStart);
        }

        [Fact]
        public void CanStartIfGpsIsInitializedAndTrackIsSelected()
        {
            _tracksService.SelectedRink.Returns(new Rink(RinkTests.EindhovenStart, RinkTests.EindhovenFinish,RinkId));
            _sut.AttachHandlers();
            _locationService.LocationReceived += Raise.EventWith(null, new CoordinateEventArgs(_locationStub));

            Assert.True(_sut.CanStart);
        }

        [Fact]
        public void CanStartIsUpdatedWhenTrackIsSelected()
        {
            _tracksService.SelectedRink.Returns(new Rink(RinkTests.EindhovenStart, RinkTests.EindhovenFinish,RinkId));
            var canStartUpdated = 0;
            _sut.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(_sut.CanStart))
                    canStartUpdated = +1;
            };

            _sut.AttachHandlers();
            _locationService.LocationReceived += Raise.EventWith(null, new CoordinateEventArgs(_locationStub));

            Assert.Equal(1, canStartUpdated);
        }

        [Fact]
        public async Task StartCommandNavigatesToSessionPage_WhenCanStart_AndBtIsOn()
        {
            _bluetoothService.IsBluetoothAvailable().Returns(true);
            _tracksService.SelectedRink.Returns(new Rink(RinkTests.EindhovenStart, RinkTests.EindhovenFinish,RinkId));
            _sut.AttachHandlers();
            _locationService.LocationReceived += Raise.EventWith(null, new CoordinateEventArgs(_locationStub));

            _sut.StartCommand.Execute(null);

            await _navigationService.Received().NavigateToViewModelAsync<LiveSessionViewModel>();
        }

        [Fact]
        public async Task StartCommandDoesNotNavigateToSessionPage_WhenCanStart_ButBtIsOff()
        {
            _bluetoothService.IsBluetoothAvailable().Returns(false);
            _tracksService.SelectedRink.Returns(new Rink(RinkTests.EindhovenStart, RinkTests.EindhovenFinish,RinkId));
            _sut.AttachHandlers();
            _locationService.LocationReceived += Raise.EventWith(null, new CoordinateEventArgs(_locationStub));

            _sut.StartCommand.Execute(null);

            await _navigationService.DidNotReceive().NavigateToViewModelAsync<LiveSessionViewModel>();
        }

        [Fact]
        public async Task StartCommandAsksToTurnBtOn_WhenBtIsOff()
        {
            _bluetoothService.IsBluetoothAvailable().Returns(false);

            _sut.StartCommand.Execute(null);

            await _bluetoothService.Received().EnableBluetoothAsync();
        }

        [Fact]
        public async Task StartCommandDoesNotAskToTurnBtOn_WhenBtIsOn()
        {
            _bluetoothService.IsBluetoothAvailable().Returns(true);

            _sut.StartCommand.Execute(null);

            await _bluetoothService.DidNotReceive().EnableBluetoothAsync();
        }

        [Fact]
        public async Task StartCommandDoesNotNavigateToSessionPage_WhenCannotStart()
        {
            _sut.StartCommand.Execute(null);

            await _navigationService.DidNotReceive().NavigateToViewModelAsync<LiveSessionViewModel>();
        }

        [Fact]
        public async Task SelectRinkCommandNavigatesToTracksPage_WhenBtIsAvailable_AndBleIsAllowed()
        {
            _settingsService.UseBle.Returns(true);
            _bluetoothService.IsBluetoothAvailable().Returns(true);

            _sut.SelectRinkCommand.Execute(null);

            await _navigationService.Received().NavigateToViewModelAsync<TracksViewModel>();
        }

        [Fact]
        public async Task SelectRinkCommandDoesNotAskToTurnBtOn_WhenBtIsAvailable()
        {
            _bluetoothService.IsBluetoothAvailable().Returns(true);

            _sut.SelectRinkCommand.Execute(null);

            await _bluetoothService.DidNotReceive().EnableBluetoothAsync();
        }

        [Fact]
        public async Task SelectRinkCommandDoesNotNavigatesToTracksPage_IfBluetoothIsNotOn_ButAllowed()
        {
            _settingsService.UseBle.Returns(true);
            _bluetoothService.IsBluetoothAvailable().Returns(false);
            _sut.SelectRinkCommand.Execute(null);

            await _navigationService.DidNotReceive().NavigateToViewModelAsync<TracksViewModel>();
        }

        [Fact]
        public async Task SelectRinkCommandDoesNotNavigatesToTracksPage_IfBluetoothIsNotOn_AndNotAllowed()
        {
            _settingsService.UseBle.Returns(false);
            _bluetoothService.IsBluetoothAvailable().Returns(false);
            _sut.SelectRinkCommand.Execute(null);

            await _navigationService.Received().NavigateToViewModelAsync<TracksViewModel>();
        }

        [Fact]
        public async Task SelectRinkCommandAsksToEnableBluetoothFirst_IfBluetoothIsNotOn_ButAllowed()
        {
            _settingsService.UseBle.Returns(true);
            _bluetoothService.IsBluetoothAvailable().Returns(false);
            _sut.SelectRinkCommand.Execute(null);

            await _bluetoothService.Received().EnableBluetoothAsync();
        }

        [Fact]
        public void StartCommand_Creates_NewSession_With_Selected_Rink()
        {
            _bluetoothService.IsBluetoothAvailable().Returns(true);
            var rink = new Rink(RinkTests.EindhovenStart, RinkTests.EindhovenFinish, RinkId);
            _tracksService.SelectedRink.Returns(rink);
            _sut.AttachHandlers();
            //to stop geo services init
            _locationService.LocationReceived += Raise.EventWith(null, new CoordinateEventArgs(_locationStub));

            _sut.StartCommand.Execute(null);

            _sessionProvider.Received(1).CreateSessionForRink(rink);
        }
    }
}
