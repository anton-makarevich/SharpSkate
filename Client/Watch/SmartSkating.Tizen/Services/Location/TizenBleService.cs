using System;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Dto.Services;
using Sanet.SmartSkating.Services.Location;
using Tizen.Network.Bluetooth;

namespace Sanet.SmartSkating.Tizen.Services.Location
{
    public class TizenBleService:BaseBleLocationService
    {
        private readonly IDataService _dataService;

        public TizenBleService(
            IDataService dataService,
            IBleDevicesProvider bleDevicesProvider):base(bleDevicesProvider)
        {
            _dataService = dataService;
        }
        
        public override void StartBleScan()
        {
            RunScan();
        }

        private void RunScan()
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

        public override void StopBleScan()
        {
            BluetoothAdapter.ScanResultChanged-= BluetoothAdapterOnScanResultChanged;
        }
    }
}