using System.Threading.Tasks;
using NSubstitute;
using Sanet.SmartSkating.Models;
using Sanet.SmartSkating.Models.EventArgs;
using Sanet.SmartSkating.Services.Location;
using Sanet.SmartSkating.Services.Storage;
using Sanet.SmartSkating.ViewModels;
using Xunit;

namespace Sanet.SmartSkating.Tests.ViewModels
{
    public class LiveSessionViewModelTests
    {
        private readonly LiveSessionViewModel _sut;
        private readonly ILocationService _locationService = Substitute.For<ILocationService>();
        private readonly IStorageService _storageService = Substitute.For<IStorageService>();
        private readonly Coordinate _locationStub = new Coordinate(23, 45);

        public LiveSessionViewModelTests()
        {
            _sut = new LiveSessionViewModel(_locationService,_storageService);
        }

        [Fact]
        public void StartsLocationServiceWhenStartButtonPressed()
        {
            _sut.StartCommand.Execute(null);

            _locationService.Received().StartFetchLocation();
        }

        [Fact]
        public void StopsLocationServiceWhenStopButtonPressed()
        {
            _sut.StopCommand.Execute(null);

            _locationService.Received().StopFetchLocation();
        }

        [Fact]
        public void ChangesStateToIsRunningWhenStartButtonPressed()
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
        public void ChangesStateToNotIsRunningWhenStartButtonPressed()
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
        public void StopsLocationServiceWhenLeavingThePage()
        {
            _sut.StartCommand.Execute(null);

            _sut.DetachHandlers();

            _locationService.Received().StopFetchLocation();
            Assert.False(_sut.IsRunning);
        }

        [Fact]
        public void UpdatesLastLocationWithNewValueFromServiceIfServiceIsStarted()
        {
            _sut.StartCommand.Execute(null);
            _locationService.LocationReceived += Raise.EventWith(null, new CoordinateEventArgs(_locationStub));

            Assert.Equal(_locationStub, _sut.LastCoordinate);
        }

        [Fact]
        public void LastCoordinateChangeUpdatesInfoLabel()
        {
            _sut.StartCommand.Execute(null);
            _locationService.LocationReceived += Raise.EventWith(null, new CoordinateEventArgs(_locationStub));

            Assert.Contains(_sut.InfoLabel, _locationStub.ToString());
        }

        [Fact]
        public void StopClearsInfoLabel()
        {
            _sut.StartCommand.Execute(null);
            _locationService.LocationReceived += Raise.EventWith(null, new CoordinateEventArgs(_locationStub));

            _sut.StopCommand.Execute(true);

            Assert.Empty(_sut.InfoLabel);
        }

        [Fact]
        public async Task LastCoordinateChangeSavesCoordinateToDisk()
        {
            _sut.StartCommand.Execute(null);
            _locationService.LocationReceived += Raise.EventWith(null, new CoordinateEventArgs(_locationStub));

            await _storageService.Received().SaveCoordinateAsync(_locationStub);
        }
    }
}