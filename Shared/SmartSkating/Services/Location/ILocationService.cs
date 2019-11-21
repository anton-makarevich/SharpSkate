using System;
using Sanet.SmartSkating.Models.EventArgs;

namespace Sanet.SmartSkating.Services.Location
{
    public interface ILocationService
    {
        event EventHandler<CoordinateEventArgs>? LocationReceived;
        void StartFetchLocation();
        void StopFetchLocation();
    }
}