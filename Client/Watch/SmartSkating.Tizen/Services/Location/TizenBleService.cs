using System;
using System.Threading.Tasks;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Dto.Services;
using Sanet.SmartSkating.Models.EventArgs;
using Sanet.SmartSkating.Services.Location;
using Tizen.Network.Bluetooth;

namespace Sanet.SmartSkating.Tizen.Services.Location
{
    public class TizenBleService:IBleLocationService
    {
        private readonly IDataService _dataService;
        public event EventHandler<CheckPointEventArgs>? CheckPointPassed;

        public TizenBleService(IDataService dataService)
        {
            _dataService = dataService;
        }
        
        public void StartBleScan()
        {
            RunScan();
        }

        private async Task RunScan()
        {
            BluetoothAdapter.ScanResultChanged+= BluetoothAdapterOnScanResultChanged;
        }

        private void BluetoothAdapterOnScanResultChanged(object sender, AdapterLeScanResultChangedEventArgs e)
        {
            if (e.DeviceData == null || e.DeviceData.DeviceName != "RDL51822") return;
            var bleDto = new BleScanResultDto
            {
                DeviceAddress = e.DeviceData.RemoteAddress,
                Id = Guid.NewGuid().ToString("N"),
                Rssi = e.DeviceData.Rssi,
                Time = DateTime.UtcNow
            };
            _dataService.SaveBleAsync(bleDto);
        }

        public void StopBleScan()
        {
            
            BluetoothAdapter.ScanResultChanged-= BluetoothAdapterOnScanResultChanged;
        }
    }
}