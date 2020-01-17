using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.Bluetooth;
using Android.Bluetooth.LE;
using BleWriter.Android.Models;
using BleWriter.Services;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Models.EventArgs;
using ScanMode = Android.Bluetooth.LE.ScanMode;

namespace BleWriter.Android.Services
{
    public class AndroidBleWriter:IBleWriterService
    {
        private readonly BleWriterScanCallBack _callBack = new BleWriterScanCallBack();
        private readonly BluetoothLeScanner _bleScanner= BluetoothAdapter.DefaultAdapter.BluetoothLeScanner;
        
        private readonly List<BluetoothDevice> _knownDevices = new List<BluetoothDevice>();

        private const string DeviceName = "DeviceToScsnFor";
        
        public Task WriteDeviceIdAsync(BleDeviceDto deviceStub)
        {
            throw new System.NotImplementedException();
        }

        public void StopBleScan()
        {
            _bleScanner.StopScan(_callBack);
            _bleScanner.FlushPendingScanResults(_callBack);
            _callBack.BeaconFound -= CallBackOnBeaconFound;
        }

        public event EventHandler<BleDeviceEventArgs>? NewBleDeviceFound;
        public void StartBleScan()
        {
            if (_knownDevices == null || _knownDevices.Count == 0)
                return;
            _callBack.BeaconFound += CallBackOnBeaconFound;

            var filter = new ScanFilter.Builder()
                    .SetDeviceName(DeviceName)
                    .Build();

            var settings = new ScanSettings.Builder()
                .SetScanMode(ScanMode.LowLatency)
                .SetMatchMode(BluetoothScanMatchMode.Aggressive)
                .SetNumOfMatches(1)
                .Build();
            _bleScanner.StartScan(
                new List<ScanFilter>(){filter},
                settings,
                _callBack);
        }

        private void CallBackOnBeaconFound(object sender, BluetoothDevice e)
        {
            var existing = _knownDevices.FirstOrDefault(f => f.Address == e.Address);
            if (existing != null) return;
            _knownDevices.Add(e);
            NewBleDeviceFound?.Invoke(this, new BleDeviceEventArgs(new BleDeviceDto
            {
                DeviceName = DeviceName,
                Id = e.Address
            }));
        }
    }
}