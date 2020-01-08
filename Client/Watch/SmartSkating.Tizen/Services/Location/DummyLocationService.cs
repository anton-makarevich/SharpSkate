using System;
using System.Threading.Tasks;
using GpxTools.Services;
using Sanet.SmartSkating.Models.EventArgs;
using Sanet.SmartSkating.Models.Location;
using Sanet.SmartSkating.Services.Location;

namespace Sanet.SmartSkating.Tizen.Services.Location
{
    public class DummyLocationService:ILocationService
    {
        private readonly GpxReaderService _gpxReader = new GpxReaderService();

        private readonly string _dummyLocation;
        private int _delay;
        
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
            DateTime? prevTime = null;
            while (_isRunning)
            {
                foreach (var routePoint in route.RoutePoints)
                {
                    if (prevTime != null && routePoint.Time!=null && _delay<1)
                    {
                        _delay = (int)routePoint.Time.Value.Subtract(prevTime.Value).TotalMilliseconds;
                    }

                    prevTime = routePoint.Time;
                    if (!_isRunning)
                        return;
                    LocationReceived?.Invoke(
                        this, 
                        new CoordinateEventArgs(
                            new Coordinate(
                                routePoint.Latitude,
                                routePoint.Longitude),
                            routePoint.Time));
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