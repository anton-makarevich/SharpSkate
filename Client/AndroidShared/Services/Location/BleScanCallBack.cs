using System;
using System.Collections.Generic;
using Android.Bluetooth.LE;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Dto.Services;

namespace Sanet.SmartSkating.Droid.Services.Location
{
    class BleScanCallBack : ScanCallback
    {
        private readonly IDataService _dataService;
        public event EventHandler<BleScanResultDto>? BeaconFound;

        public BleScanCallBack(IDataService dataService)
        {
            _dataService = dataService;
        }

        public override void OnScanResult(ScanCallbackType callbackType, ScanResult result)
        {
            base.OnScanResult(callbackType, result);
            if (callbackType!= ScanCallbackType.MatchLost) 
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
            var bleDto = new BleScanResultDto
            {
                DeviceAddress = result.Device.Address,
                Id = Guid.NewGuid().ToString("N"),
                Rssi = result.Rssi,
                Time = DateTime.UtcNow
            };
            _dataService.SaveBleScanAsync(bleDto);
            BeaconFound?.Invoke(this,bleDto);
        }
    }
}