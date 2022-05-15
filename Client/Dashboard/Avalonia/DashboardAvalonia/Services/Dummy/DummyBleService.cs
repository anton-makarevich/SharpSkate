using System;
using System.Threading.Tasks;
using Sanet.SmartSkating.Models.EventArgs;
using Sanet.SmartSkating.Services.Location;

namespace Sanet.SmartSkating.Dashboard.Avalonia.Services.Dummy
{
    class DummyBleService : IBleLocationService
    {
        public event EventHandler<CheckPointEventArgs>? CheckPointPassed;

        public async Task LoadDevicesDataAsync()
        {
        }

        public void StartBleScan(string sessionId)
        {
        }

        public void StopBleScan()
        {
        }
    }
}
