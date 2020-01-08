using System;
using System.Collections.Generic;
using System.Linq;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Dto.Services;
using Sanet.SmartSkating.Services.Location;
using Tizen.Network.Bluetooth;

namespace Sanet.SmartSkating.Tizen.Services.Location
{
    public class TizenBleService:BaseBleLocationService
    {
        private readonly IDataService _dataService;
        private List<string> _allowedDeviceNames;
        private List<string> _allowedDeviceIds;
        

        public TizenBleService(
            IDataService dataService,
            IBleDevicesProvider bleDevicesProvider):base(bleDevicesProvider)
        {
            _dataService = dataService;
        }
        
        public override void StartBleScan()
        {
            if (KnownDevices==null || KnownDevices.Count == 0)
                return;

            _allowedDeviceNames = KnownDevices.Select(d => d.DeviceName).ToList();
            _allowedDeviceIds = KnownDevices.Select(d => d.Id).ToList();
            
            base.StartBleScan();
            RunScan();
        }

        private void RunScan()
        {
            BluetoothAdapter.ScanResultChanged+= BluetoothAdapterOnScanResultChanged;
            BluetoothAdapter.StartLeScan();
        }

        private void BluetoothAdapterOnScanResultChanged(object sender, AdapterLeScanResultChangedEventArgs e)
        {
            if (e.DeviceData == null 
                || !_allowedDeviceNames.Contains(e.DeviceData.DeviceName)
                || !_allowedDeviceIds.Contains(e.DeviceData.RemoteAddress)
                ) return;
            var bleDto = new BleScanResultDto
            {
                DeviceAddress = e.DeviceData.RemoteAddress,
                Id = Guid.NewGuid().ToString("N"),
                Rssi = e.DeviceData.Rssi,
                Time = DateTime.UtcNow
            };
            _dataService.SaveBleAsync(bleDto);
            ProceedNewScan(bleDto);
        }

        public override void StopBleScan()
        {
            base.StopBleScan();
            BluetoothAdapter.ScanResultChanged-= BluetoothAdapterOnScanResultChanged;
            BluetoothAdapter.StopLeScan();
        }
    }
}