using System;
using System.Collections.Generic;
using System.Text;
using Android.Bluetooth;
using Android.Bluetooth.LE;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Dto.Services;
using Sanet.SmartSkating.Models.EventArgs;
using Sanet.SmartSkating.Services.Location;

namespace Sanet.SmartSkating.Droid.Services.Location
{
    class BleScanCallBack : ScanCallback
    {
        private readonly IDataService _dataService;

        public StringBuilder DCLog { get; }
        public StringBuilder C9Log { get; }
        
        public BleScanCallBack(IDataService dataService)
        {
            _dataService = dataService;
            DCLog = new StringBuilder();
            C9Log = new StringBuilder();
        }

        public override void OnScanResult(ScanCallbackType callbackType, ScanResult result)
        {
            base.OnScanResult(callbackType, result);
            ProcessScanResult(result);
        }

        public override void OnScanFailed(ScanFailure errorCode)
        {
            base.OnScanFailed(errorCode);
            Console.WriteLine($"Scan failed :=) Error code: {errorCode}");
        }

        public override void OnBatchScanResults(IList<ScanResult> results)
        {
            base.OnBatchScanResults(results);
            foreach (var result in results)
            {
                ProcessScanResult(result);
            }
        }

        private void ProcessScanResult(ScanResult result)
        {
            if (result.Device.Name != "RDL51822") return;
            var bleDto = new BleScanResultDto
            {
                DeviceAddress = result.Device.Address,
                Id = Guid.NewGuid().ToString("N"),
                Rssi = result.Rssi,
                Time = DateTime.UtcNow
            };
            _dataService.SaveBleAsync(bleDto);
        }
    }
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