using System;
using System.Threading.Tasks;
using GpxTools.Services;
using Sanet.SmartSkating.Models;
using Sanet.SmartSkating.Models.EventArgs;
using Sanet.SmartSkating.Services.Location;

namespace Sanet.SmartSkating.Tizen.Services
{
    public class DummyLocationService:ILocationService
    {
        private readonly GpxReaderService _gpxReader = new GpxReaderService();
        
        public event EventHandler<CoordinateEventArgs>? LocationReceived;
        private bool _isRunning;
        public void StartFetchLocation()
        {
            _isRunning = true;
#pragma warning disable 4014
            RunMockLocations();
#pragma warning restore 4014
        }

        private async Task RunMockLocations()
        {
            var route = await _gpxReader.ReadEmbeddedGpxFileAsync("Grefrath");
            while (_isRunning)
            {
                foreach (var routePoint in route.RoutePoints)
                {
                    if (!_isRunning)
                        return;
                    LocationReceived?.Invoke(
                        this, 
                        new CoordinateEventArgs(
                            new Coordinate(
                                routePoint.Latitude,
                                routePoint.Longitude)));
                    await Task.Delay(3000);
                }
            }
        }

        public void StopFetchLocation()
        {
            _isRunning = false;
        }
    }
}