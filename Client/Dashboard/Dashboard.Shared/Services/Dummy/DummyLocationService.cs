using System;
using Sanet.SmartSkating.Models.EventArgs;
using Sanet.SmartSkating.Services.Location;

namespace Sanet.SmartSkating.Dashboard.Services.Dummy
{
    public class DummyLocationService:ILocationService
    {
        public event EventHandler<CoordinateEventArgs>? LocationReceived;
        public void StartFetchLocation()
        {
        }

        public void StopFetchLocation()
        {
        }
    }
}