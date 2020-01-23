using System;
using System.Threading.Tasks;
using Sanet.SmartSkating.Models.EventArgs;

namespace Sanet.SmartSkating.Services.Location
{
    public interface IBleLocationService
    {
        event EventHandler<CheckPointEventArgs>? CheckPointPassed;
        void StartBleScan(string sessionId);
        void StopBleScan();
        Task LoadDevicesDataAsync();
    }
}