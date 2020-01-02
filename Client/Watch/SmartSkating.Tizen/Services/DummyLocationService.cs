using System;
using System.Threading.Tasks;
using GpxTools.Services;
using Sanet.SmartSkating.Models.EventArgs;
using Sanet.SmartSkating.Models.Location;
using Sanet.SmartSkating.Services.Location;

namespace Sanet.SmartSkating.Tizen.Services
{
    public class DummyLocationService:ILocationService
    {
        private readonly GpxReaderService _gpxReader = new GpxReaderService();

        private readonly string _dummyLocation;
        private readonly int _delay;
        
        public event EventHandler<CoordinateEventArgs>? LocationReceived;
        private bool _isRunning;

        public DummyLocationService(string location, int delay)
        {
            _dummyLocation = location;
            _delay = delay;
        }
        
        public void StartFetchLocation()
        {
            _isRunning = true;
#pragma warning disable 4014
            RunMockLocations();
#pragma warning restore 4014
        }

        private async Task RunMockLocations()
        {
            var route = await _gpxReader.ReadEmbeddedGpxFileAsync(_dummyLocation);
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
                    await Task.Delay(_delay);
                }
                Console.WriteLine("End of file!");
            }
        }

        public void StopFetchLocation()
        {
            _isRunning = false;
        }
    }
}