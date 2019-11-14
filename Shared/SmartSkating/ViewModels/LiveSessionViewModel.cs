using System.Windows.Input;
using Sanet.SmartSkating.Models;
using Sanet.SmartSkating.Models.EventArgs;
using Sanet.SmartSkating.Service.Location;
using Sanet.SmartSkating.ViewModels.Base;

namespace Sanet.SmartSkating.ViewModels
{
    public class LiveSessionViewModel:BaseViewModel
    {
        private readonly ILocationService _locationService;
        private bool _isRunning;
        private string _infoLabel;

        public LiveSessionViewModel(ILocationService locationService)
        {
            _locationService = locationService;
        }


        public ICommand StartCommand => new SimpleCommand(() =>
        {
            _locationService.LocationReceived+= LocationServiceOnLocationReceived;
            _locationService.StartFetchLocation();
            _isRunning = true;
        });

        private void LocationServiceOnLocationReceived(object sender, CoordinateEventArgs e)
        {
            LastCoordinate = e.Coordinate;
            InfoLabel = LastCoordinate.ToString();
        }

        public ICommand StopCommand => new SimpleCommand(StopLocationService);

        private void StopLocationService()
        {
            _locationService.LocationReceived -= LocationServiceOnLocationReceived;
            _locationService.StopFetchLocation();
            _isRunning = false;
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

        public override void DetachHandlers()
        {
            base.DetachHandlers();
            StopLocationService();
        }
    }
}