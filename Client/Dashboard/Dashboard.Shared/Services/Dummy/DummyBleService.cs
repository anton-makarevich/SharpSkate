using Sanet.SmartSkating.Models.EventArgs;
using Sanet.SmartSkating.Services.Location;
using System;
using System.Threading.Tasks;

namespace Sanet.SmartSkating.Dashboard.Services.Dummy
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
