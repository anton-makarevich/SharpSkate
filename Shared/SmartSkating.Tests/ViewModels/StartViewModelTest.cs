using NSubstitute;
using Sanet.SmartSkating.Models;
using Sanet.SmartSkating.Models.EventArgs;
using Sanet.SmartSkating.Models.Geometry;
using Sanet.SmartSkating.Services.Location;
using Sanet.SmartSkating.Services.Tracking;
using Sanet.SmartSkating.Tests.Models.Geometry;
using Sanet.SmartSkating.ViewModels;
using Xunit;

namespace Sanet.SmartSkating.Tests.ViewModels
{
    public class StartViewModelTest
    {
        private readonly StartViewModel _sut;
        private readonly ILocationService _locationService;
        private readonly Coordinate _locationStub = new Coordinate(23, 45);
        private readonly ITrackService _tracksService;

        public StartViewModelTest()
        {
            _tracksService = Substitute.For<ITrackService>();
            _locationService = Substitute.For<ILocationService>();
            _sut = new StartViewModel(_locationService,_tracksService);
        }

        [Fact]
        public void MakesGeoLocationRequestOnPageAppear()
        {
            _sut.AttachHandlers();
            
            _locationService.Received().StartFetchLocation();
        }

        [Fact]
        public void StopsGeoLocationFetchingAfterCoordinateHasBeenReceived()
        {
            _sut.AttachHandlers();
            _locationService.LocationReceived += Raise.EventWith(null, new CoordinateEventArgs(_locationStub));
            
            _locationService.Received().StopFetchLocation();
        }

        [Fact]
        public void ShowsInformationLabelDuringGeoFetching()
        {
            _sut.AttachHandlers();
            
            Assert.Equal("Initializing GeoServices. Be sure you're in open air", _sut.InfoLabel);
        }
        
        [Fact]
        public void GeoServicesAreInitializedWhenCoordinateHasBeenReceived()
        {
            _sut.AttachHandlers();
            _locationService.LocationReceived += Raise.EventWith(null, new CoordinateEventArgs(_locationStub));
            
            Assert.True(_sut.GeoServicesAreInitialized);
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
            _tracksService.SelectedRink.Returns(new Rink(RinkTests.EindhovenStart, RinkTests.EindhovenFinish,""));
            
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
            _tracksService.SelectedRink.Returns(new Rink(RinkTests.EindhovenStart, RinkTests.EindhovenFinish,""));
            
            _sut.AttachHandlers();
            _locationService.LocationReceived += Raise.EventWith(null, new CoordinateEventArgs(_locationStub));
            
            Assert.True(_sut.IsRinkSelected);
        }
        
        [Fact]
        public void IsTrackSelectedIsUpdatedWhenCoordinateHasBeenReceived()
        {
            _tracksService.SelectedRink.Returns(new Rink(RinkTests.EindhovenStart, RinkTests.EindhovenFinish,""));
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
            _tracksService.SelectedRink.Returns(new Rink(RinkTests.EindhovenStart, RinkTests.EindhovenFinish,trackName));
            
            _sut.AttachHandlers();
            _locationService.LocationReceived += Raise.EventWith(null, new CoordinateEventArgs(_locationStub));
            
            Assert.Equal(trackName,_sut.RinkName);
        }
        
        [Fact]
        public void TrackNameIsUpdatedWhenTrackIsSelected()
        {
            _tracksService.SelectedRink.Returns(new Rink(RinkTests.EindhovenStart, RinkTests.EindhovenFinish,""));
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
    }
}