using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NSubstitute;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Services;
using Sanet.SmartSkating.Services.Tracking;
using Sanet.SmartSkating.Tests.Services.Tracking;
using Sanet.SmartSkating.ViewModels;
using Sanet.SmartSkating.ViewModels.Wrappers;
using Xunit;

namespace Sanet.SmartSkating.Tests.ViewModels
{
    public class TracksViewModelTests
    {
        private readonly ITrackService _trackServiceMock = Substitute.For<ITrackService>();
        private readonly INavigationService _navigationServiceMock = Substitute.For<INavigationService>();
        private readonly TracksViewModel _sut;

        public TracksViewModelTests()
        {
            _sut = new TracksViewModel(_trackServiceMock, _navigationServiceMock);
            var tracks = JsonConvert.DeserializeObject<List<TrackDto>>(TrackServiceTests.TracksData);
            _trackServiceMock.Tracks.Returns(tracks);
        }

        [Fact]
        public async Task LoadTracksFromService()
        {
            await _sut.LoadTracksAsync();

            await _trackServiceMock.Received().LoadTracksAsync();
            Assert.NotEmpty(_sut.Tracks);
        }

        [Fact]
        public async Task HasSelectedTrackIsFalseIfNoneOfTracksIsSelected()
        {
            await _sut.LoadTracksAsync();
            
            Assert.False(_sut.HasSelectedTrack);
        }

        [Fact]
        public async Task SelectTrackSetsItsIsSelectedToTrue()
        {
            await _sut.LoadTracksAsync();
            var track = _sut.Tracks.First();

            _sut.SelectTrack(track);
            
            Assert.True(track.IsSelected);
        }
        
        [Fact]
        public async Task SelectTrackRemovesIsSelectedFromOtherTracks()
        {
            await _sut.LoadTracksAsync();
            var track = _sut.Tracks.First();
            track.IsSelected = true;
            var secondTrack = _sut.Tracks.Last();

            _sut.SelectTrack(secondTrack);
            
            Assert.False(track.IsSelected);
        }

        [Fact]
        public async Task SelectTrackDoesNotChangeIsSelectIsTrackIsNotPartOfViewModel()
        {
            await _sut.LoadTracksAsync();
            var trackVm = new TrackViewModel(new TrackDto
            {
                Name = "SomeTrack",
                Start = new CoordinateDto{Latitude = 11,Longitude = 45},
                Finish = new CoordinateDto{Latitude = 16,Longitude = 25},
            });
            
            _sut.SelectTrack(trackVm);
            
            Assert.False(trackVm.IsSelected);
        }

        [Fact]
        public async Task HasSelectedTrackIsTrueWhenOneTrackIsSelected()
        {
            await _sut.LoadTracksAsync();
            var track = _sut.Tracks.First();

            _sut.SelectTrack(track);
            
            Assert.True(_sut.HasSelectedTrack);
        }

        [Fact]
        public async Task SelectTrackRaisesPropertyChangedForHasSelectedTrack()
        {
            await _sut.LoadTracksAsync();
            var track = _sut.Tracks.First();
            var hasSelectedUpdatedTimes = 0;
            _sut.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(_sut.HasSelectedTrack))
                    hasSelectedUpdatedTimes++;
            };

            _sut.SelectTrack(track);
            
            Assert.Equal(1,hasSelectedUpdatedTimes);
        }

        [Fact]
        public async Task SelectTrackUpdatesCurrentRinkInService()
        {
            await _sut.LoadTracksAsync();
            var track = _sut.Tracks.First();

            _sut.SelectTrack(track);
            
            _trackServiceMock.Received().SelectRinkByName(track.Name);
        }

        [Fact]
        public async Task ConfirmingSelectionCallsNavigationIfTrackIsSelected()
        {
            await _sut.LoadTracksAsync();
            var track = _sut.Tracks.First();

            _sut.SelectTrack(track);

            _sut.ConfirmSelectionCommand.Execute(null);

            await _navigationServiceMock.Received().NavigateToViewModelAsync<LiveSessionViewModel>();
        }
        
        [Fact]
        public async Task ConfirmingSelectionDoesNotCallNavigationIfTrackIsNotSelected()
        {
            await _sut.LoadTracksAsync();
            
            _sut.ConfirmSelectionCommand.Execute(null);

            await _navigationServiceMock.DidNotReceive().NavigateToViewModelAsync<LiveSessionViewModel>();
        }
    }
}