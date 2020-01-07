using System.Collections.Generic;
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

        public AndroidBleService(IDataService dataService, IBleDevicesProvider bleDevicesProvider):base(bleDevicesProvider)
        {
            _callBack = new BleScanCallBack(dataService);
            _bleScanner = BluetoothAdapter.DefaultAdapter.BluetoothLeScanner;
        }
        
        public override void StartBleScan()
        {
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
                    InvokeCheckPointPassed(WayPointTypes.Start, e.Time);
                    break;
                case "DC:E6:99:19:95:41":
                    InvokeCheckPointPassed(WayPointTypes.Finish, e.Time);
                    break;
            }
        }

        public override void StopBleScan()
        {
            _bleScanner.StopScan(_callBack);
            _bleScanner.FlushPendingScanResults(_callBack);
            _callBack.BeaconFound -= OnBeaconFound;
        }
    }
}