using System;
using System.Collections.Generic;
using Android.Bluetooth;
using Android.Bluetooth.LE;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Dto.Services;
using Sanet.SmartSkating.Models.EventArgs;
using Sanet.SmartSkating.Models.Training;
using Sanet.SmartSkating.Services.Location;
using ScanMode = Android.Bluetooth.LE.ScanMode;

namespace Sanet.SmartSkating.Droid.Services.Location
{
    public class AndroidBleService:IBleLocationService
    {
        private readonly IDataService _dataService;
        private readonly BluetoothLeScanner _bleScanner;
        private BleScanCallBack _callBack;

        public AndroidBleService(IDataService dataService)
        {
            _dataService = dataService;
            _bleScanner = BluetoothAdapter.DefaultAdapter.BluetoothLeScanner;
        }

        public event EventHandler<CheckPointEventArgs>? CheckPointPassed;
        public void StartBleScan()
        {
            _callBack = new BleScanCallBack(_dataService);
            
            _callBack.BeaconFound += OnBeaconFound;
            
            var filter = new ScanFilter.Builder()
                .SetDeviceName("RDL51822")
                .Build();
            var settings = new ScanSettings.Builder()
                .SetScanMode(ScanMode.LowLatency)
                .SetMatchMode(BluetoothScanMatchMode.Aggressive)
                .SetNumOfMatches(1)
                .Build();
            _bleScanner.StartScan(
                new List<ScanFilter>(){filter}
                ,settings,
                _callBack);
        }

        private void OnBeaconFound(object sender, BleScanResultDto e)
        {
            switch (e.DeviceAddress)
            {
                case "C9:97:BF:3A:FE:41":
                    CheckPointPassed?.Invoke(this,new CheckPointEventArgs(
                        WayPointTypes.Start, e.Time));
                    break;
                case "DC:E6:99:19:95:41":
                    CheckPointPassed?.Invoke(this,new CheckPointEventArgs(
                        WayPointTypes.Finish, e.Time));
                    break;
            }
        }

        public void StopBleScan()
        {
            _bleScanner.StopScan(_callBack);
            _bleScanner.FlushPendingScanResults(_callBack);
            _callBack.BeaconFound -= OnBeaconFound;

        }
    }
}