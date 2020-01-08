using System;
using System.Linq;
using Android.Bluetooth;
using Android.Bluetooth.LE;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Dto.Services;
using Sanet.SmartSkating.Services.Location;
using ScanMode = Android.Bluetooth.LE.ScanMode;

namespace Sanet.SmartSkating.Droid.Services.Location
{
    public class AndroidBleService:BaseBleLocationService
    {
        private readonly BluetoothLeScanner _bleScanner;
        private readonly BleScanCallBack _callBack;

        private DateTime _startTime;

        public AndroidBleService(IDataService dataService, IBleDevicesProvider bleDevicesProvider):base(bleDevicesProvider)
        {
            _callBack = new BleScanCallBack(dataService);
            _bleScanner = BluetoothAdapter.DefaultAdapter.BluetoothLeScanner;
        }
        
        public override void StartBleScan()
        {
            if (KnownDevices == null || KnownDevices.Count == 0)
                return;
            base.StartBleScan();
            _callBack.BeaconFound += OnBeaconFound;

            var filters = KnownDevices
                .Select(f => new ScanFilter.Builder().SetDeviceName(f.DeviceName)
                    .Build())
                .ToList();

            var settings = new ScanSettings.Builder()
                .SetScanMode(ScanMode.LowLatency)
                .SetMatchMode(BluetoothScanMatchMode.Aggressive)
                .SetNumOfMatches(1)
                .Build();
            _bleScanner.StartScan(
                filters,
                settings,
                _callBack);
            
            _startTime = DateTime.Now;
        }

        private void OnBeaconFound(object sender, BleScanResultDto e)
        {
            ProceedNewScan(e);
            if (DateTime.Now.Subtract(_startTime).TotalMinutes > 25)
                RestartScan(); // We need this because Android automatically stops scans that last for more than 30 mins
        }

        private void RestartScan()
        {
            _bleScanner.StopScan(_callBack);
            _callBack.BeaconFound -= OnBeaconFound;
            if (IsScanning)
                StartBleScan();
        }

        public override void StopBleScan()
        {
            base.StopBleScan();
            _bleScanner.StopScan(_callBack);
            _bleScanner.FlushPendingScanResults(_callBack);
            _callBack.BeaconFound -= OnBeaconFound;
        }
    }
}