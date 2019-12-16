using System.Windows.Input;
using Sanet.SmartSkating.Models;
using Sanet.SmartSkating.Models.EventArgs;
using Sanet.SmartSkating.Models.Training;
using Sanet.SmartSkating.Services.Location;
using Sanet.SmartSkating.Services.Storage;
using Sanet.SmartSkating.Services.Tracking;
using Sanet.SmartSkating.ViewModels.Base;

namespace Sanet.SmartSkating.ViewModels
{
    public class LiveSessionViewModel:BaseViewModel
    {
        private readonly ILocationService _locationService;
        private readonly IStorageService _storageService;
        private readonly ITrackService _trackService;
        private ISession _currentSession;
        private bool _isRunning;
        private string _infoLabel = string.Empty;
        private int _lapsCount;
        private string _currentSector;

        public LiveSessionViewModel(
            ILocationService locationService, 
            IStorageService storageService,
            ITrackService trackService)
        {
            _locationService = locationService;
            _storageService = storageService;
            _trackService = trackService;
        }
        
        public ICommand StartCommand => new SimpleCommand( () =>
        {
            _locationService.LocationReceived+= LocationServiceOnLocationReceived;
            _locationService.StartFetchLocation();
            IsRunning = true;
        });

        private void LocationServiceOnLocationReceived(object sender, CoordinateEventArgs e)
        {
            LastCoordinate = e.Coordinate;
            InfoLabel = LastCoordinate.ToString();
            _storageService.SaveCoordinateAsync(LastCoordinate);
        }

        public ICommand StopCommand => new SimpleCommand(StopLocationService);

        private void StopLocationService()
        {
            _locationService.LocationReceived -= LocationServiceOnLocationReceived;
            _locationService.StopFetchLocation();
            IsRunning = false;
            InfoLabel = string.Empty;
        }

        public bool IsRunning
        {
            get => _isRunning;
            private set => SetProperty(ref _isRunning, value);
        }

        public Coordinate LastCoordinate { get; private set; }

        public string InfoLabel
        {
            get => _infoLabel;
            private set => SetProperty(ref _infoLabel, value);
        }

        public int LapsCount
        {
            get => _lapsCount;
            set => SetProperty(ref _lapsCount, value);
        }

        public string CurrentSector
        {
            get => _currentSector;
            set => SetProperty(ref _currentSector, value);
        }

        public override void DetachHandlers()
        {
            base.DetachHandlers();
            StopLocationService();
        }
    }
}