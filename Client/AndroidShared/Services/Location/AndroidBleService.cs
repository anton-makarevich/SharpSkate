using System;
using System.Linq;
using System.Threading.Tasks;
using Android.Bluetooth;
using Android.Bluetooth.LE;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Dto.Services;
using Sanet.SmartSkating.Models.EventArgs;
using Sanet.SmartSkating.Services.Location;
using ScanMode = Android.Bluetooth.LE.ScanMode;

namespace Sanet.SmartSkating.Droid.Services.Location
{
    public class AndroidBleService:BaseBleLocationService
    {
        private readonly BluetoothLeScanner? _bleScanner;
        private readonly BleScanCallBack _callBack;

        private const int TimeToRestartBleScanMinutes = 4;
        private const int BleScanRestartTimeoutSeconds = 5;

        private DateTime _startTime;

        public AndroidBleService(
            IDataService dataService, 
            IBleDevicesProvider bleDevicesProvider):base(bleDevicesProvider,dataService)
        {
            _callBack = new BleScanCallBack(dataService);
            if (BluetoothAdapter.DefaultAdapter != null)
            {
                _bleScanner = BluetoothAdapter.DefaultAdapter.BluetoothLeScanner;
            }
        }
        
        public override void StartBleScan(string sessionId)
        {
            if (KnownDevices == null || KnownDevices.Count == 0)
                return;
            base.StartBleScan(sessionId);
            StartAndroidBleScan();
        }

        private void StartAndroidBleScan()
        {
            _callBack.BeaconFound += OnBeaconFound;
            CheckPointPassed += OnCheckPointPassed;

            var filters = KnownDevices
                .Select(f => new ScanFilter.Builder().SetDeviceName(f.DeviceName)
                    .Build())
                .ToList();

            var settings = new ScanSettings.Builder()
                .SetScanMode(ScanMode.LowLatency)
                .SetMatchMode(BluetoothScanMatchMode.Aggressive)
                .SetNumOfMatches(1)
                .Build();
            _bleScanner?.StartScan(
                filters,
                settings,
                _callBack);

            _startTime = DateTime.Now;
        }

        private async void OnCheckPointPassed(object sender, CheckPointEventArgs e)
        {
            if (!(DateTime.Now.Subtract(_startTime).TotalMinutes > TimeToRestartBleScanMinutes)) return;
            await Task.Delay(1000 * BleScanRestartTimeoutSeconds);
            RestartScan(); // We need this because Android automatically stops scans that running for more than 5/30 mins
        }

        private void OnBeaconFound(object sender, BleScanResultDto e)
        {
            ProceedNewScan(e);
        }

        private void RestartScan()
        {
            _bleScanner?.StopScan(_callBack);
            _callBack.BeaconFound -= OnBeaconFound;
            CheckPointPassed-= OnCheckPointPassed;
            if (IsScanning)
                StartAndroidBleScan();
        }

        public override void StopBleScan()
        {
            base.StopBleScan();
            _bleScanner?.StopScan(_callBack);
            _bleScanner?.FlushPendingScanResults(_callBack);
            _callBack.BeaconFound -= OnBeaconFound;
            CheckPointPassed -= OnCheckPointPassed;
        }
    }
}