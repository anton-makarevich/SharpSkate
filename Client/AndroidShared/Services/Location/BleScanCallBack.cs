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
        private readonly IDateProvider _dateProvider;
        public event EventHandler<BleScanResultDto>? BeaconFound;

        public BleScanCallBack(IDataService dataService, IDateProvider dateProvider)
        {
            _dataService = dataService;
            _dateProvider = dateProvider;
        }

        public override void OnScanResult(ScanCallbackType callbackType, ScanResult? result)
        {
            base.OnScanResult(callbackType, result);
            if (callbackType!= ScanCallbackType.MatchLost && result!=null) 
                ProcessScanResult(result);
        }

        public override void OnScanFailed(ScanFailure errorCode)
        {
            base.OnScanFailed(errorCode);
            Console.WriteLine($"Scan failed :=) Error code: {errorCode}");
        }

        public override void OnBatchScanResults(IList<ScanResult>? results)
        {
            base.OnBatchScanResults(results);
            if (results == null) return;
            foreach (var result in results)
            {
                ProcessScanResult(result);
            }
        }

        private void ProcessScanResult(ScanResult result)
        {
            if (result.Device?.Address == null) return;
            var bleDto = new BleScanResultDto
            {
                DeviceAddress = result.Device.Address,
                Id = Guid.NewGuid().ToString("N"),
                Rssi = result.Rssi,
                Time = _dateProvider.Now()
            };
            _dataService.SaveBleScanAsync(bleDto);
            BeaconFound?.Invoke(this,bleDto);
        }
    }
}