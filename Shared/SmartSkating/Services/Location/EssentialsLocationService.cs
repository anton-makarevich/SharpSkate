using System;
using System.Threading.Tasks;
using Sanet.SmartSkating.Models;
using Sanet.SmartSkating.Models.EventArgs;
using Xamarin.Essentials;

namespace Sanet.SmartSkating.Services.Location
{
    public class EssentialsLocationService:ILocationService
    {
        private bool _isRunning;
        
        public event EventHandler<CoordinateEventArgs>? LocationReceived;
        public void StartFetchLocation()
        {
            _isRunning = true;
#pragma warning disable 4014
            RunLocationUpdate();
#pragma warning restore 4014
        }

        public void StopFetchLocation()
        {
            _isRunning = false;
        }

        private async Task RunLocationUpdate()
        {
            var request = new GeolocationRequest(GeolocationAccuracy.Best);
            do
            {
                var location = await Geolocation.GetLocationAsync(request);
                if (location!=null && !location.IsFromMockProvider && _isRunning)
                    LocationReceived?.Invoke(
                        null,
                        new CoordinateEventArgs(new Coordinate(location.Latitude,location.Longitude)));
                await Task.Delay(2000);
            } while (_isRunning);
        }
    }
}