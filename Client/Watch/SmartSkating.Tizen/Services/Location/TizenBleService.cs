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
        private List<string> _allowedDeviceNames = new List<string>();
        private List<string> _allowedDeviceIds = new List<string>();

        public TizenBleService(
            IDataService dataService,
            IBleDevicesProvider bleDevicesProvider):base(bleDevicesProvider,dataService)
        {
        }
        
        public override void StartBleScan(string sessionId)
        {
            if (KnownDevices==null || KnownDevices.Count == 0)
                return;

            _allowedDeviceNames = KnownDevices.Select(d => d.DeviceName).ToList();
            _allowedDeviceIds = KnownDevices.Select(d => d.Id).ToList();
            
            base.StartBleScan(sessionId);
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