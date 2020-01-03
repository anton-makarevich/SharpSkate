using System;
using Android.Bluetooth;
using Android.Bluetooth.LE;
using Sanet.SmartSkating.Dto.Services;
using Sanet.SmartSkating.Models.EventArgs;
using Sanet.SmartSkating.Services.Location;

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
            _bleScanner.StartScan(_callBack);
        }

        public void StopBleScan()
        {
            _bleScanner.StopScan(_callBack);
        }
    }
}