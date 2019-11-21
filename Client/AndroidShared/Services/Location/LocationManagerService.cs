using System;
using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Sanet.SmartSkating.Models;
using Sanet.SmartSkating.Models.EventArgs;
using Sanet.SmartSkating.Services.Location;

namespace Sanet.SmartSkating.Droid.Services.Location
{
    public class LocationManagerService: Java.Lang.Object, ILocationService, ILocationListener
    {
        private readonly LocationManager? _locationManager;

        public LocationManagerService()
        {
        }

        public LocationManagerService(Activity activity)
        {
            _locationManager = (LocationManager) activity.GetSystemService(Context.LocationService);
        }
        
        public event EventHandler<CoordinateEventArgs>? LocationReceived;
        public void StartFetchLocation()
        {
            
            _locationManager?.RequestLocationUpdates(LocationManager.GpsProvider,2000, 1, this);
        }

        public void StopFetchLocation()
        {
            _locationManager?.RemoveUpdates(this);
        }

        protected override void Dispose(bool disposing)
        {
            _locationManager?.Dispose();
            base.Dispose(disposing);
        }

        public void OnLocationChanged(Android.Locations.Location location)
        {
            LocationReceived?.Invoke(this,
                new CoordinateEventArgs(new Coordinate(location.Latitude,location.Longitude)));
        }

        public void OnProviderDisabled(string provider)
        {
            // Set flag that provider is not enabled
        }

        public void OnProviderEnabled(string provider)
        {
            // Resume listening for location updates
        }

        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
            // Add implementation
        }
    }
}