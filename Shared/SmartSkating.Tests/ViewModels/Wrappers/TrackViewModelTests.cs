using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.ViewModels.Wrappers;
using Xunit;

namespace Sanet.SmartSkating.Tests.ViewModels.Wrappers
{
    public class TrackViewModelTests
    {
        private TrackDto _model = new TrackDto
        {
            Name = "123", 
            Start = new CoordinateDto{Latitude = 11,Longitude = 12},
            Finish =  new CoordinateDto{Latitude = 13,Longitude = 14}
        };

        private readonly TrackViewModel _sut;

        public TrackViewModelTests()
        {
            _sut = new TrackViewModel(_model);
        }

        [Fact]
        public void NewTrackViewModelHasTheSameNameAsModel()
        {
            Assert.Equal(_model.Name, _sut.Name);
        }

        [Fact]
        public void TrackIsNotSelectedInitially()
        {
            Assert.False(_sut.IsSelected);
        }
    }
}