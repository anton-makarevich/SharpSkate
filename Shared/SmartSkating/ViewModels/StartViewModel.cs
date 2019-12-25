using System;
using Sanet.SmartSkating.Models;
using Sanet.SmartSkating.Models.EventArgs;
using Sanet.SmartSkating.Services.Location;
using Sanet.SmartSkating.Services.Tracking;
using Sanet.SmartSkating.ViewModels.Base;

namespace Sanet.SmartSkating.ViewModels
{
    public class StartViewModel:BaseViewModel
    {
        private readonly ILocationService _locationService;
        private readonly ITrackService _tracksService;
        private string _infoLabel = string.Empty;
        private bool _geoServicesAreInitialized;

        public StartViewModel(ILocationService locationService, ITrackService tracksService)
        {
            _locationService = locationService;
            _tracksService = tracksService;
        }

        public string InfoLabel
        {
            get => _infoLabel;
            private set => SetProperty(ref _infoLabel, value);
        }

        public bool GeoServicesAreInitialized
        {
            get => _geoServicesAreInitialized;
            private set => SetProperty(ref _geoServicesAreInitialized, value);
        }

        public bool IsRinkSelected => _tracksService.SelectedRink!=null;
        public string RinkName => _tracksService.SelectedRink!=null
            ?_tracksService.SelectedRink.Name
            :string.Empty;

        public override void AttachHandlers()
        {
            base.AttachHandlers();
            InitializeGeoServices();
            _locationService.LocationReceived+= LocationServiceOnLocationReceived;
        }

        private void InitializeGeoServices()
        {
            _locationService.StartFetchLocation();
            InfoLabel = "Initializing GeoServices. Be sure you're in open air";
        }

        private void LocationServiceOnLocationReceived(object sender, CoordinateEventArgs e)
        {
            GeoServicesAreInitialized = !e.Coordinate.Equals(default(Coordinate));
            InfoLabel = String.Empty;
            if (GeoServicesAreInitialized)
            {
                _tracksService.SelectRinkCloseTo(e.Coordinate);
                if (_tracksService.SelectedRink == null)
                    InfoLabel = "No known Rinks nearby, please select manually";
                NotifyPropertyChanged(nameof(IsRinkSelected));
                NotifyPropertyChanged(nameof(RinkName));
            }

            _locationService.StopFetchLocation();
        }

        public override void DetachHandlers()
        {
            base.DetachHandlers();
            _locationService.LocationReceived-= LocationServiceOnLocationReceived;
        }
    }
}