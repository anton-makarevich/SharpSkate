using System;
using Sanet.SmartSkating.Models;
using Sanet.SmartSkating.Models.EventArgs;

namespace Sanet.SmartSkating.Service.Location
{
    public interface ILocationService
    {
        event EventHandler<CoordinateEventArgs> LocationReceived;
        void StartFetchLocation();
        void StopFetchLocation();
    }
}