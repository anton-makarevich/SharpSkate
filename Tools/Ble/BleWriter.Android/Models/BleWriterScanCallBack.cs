using System;
using Android.Bluetooth;
using Android.Bluetooth.LE;

namespace BleWriter.Android.Models
{
    class BleWriterScanCallBack : ScanCallback
    {
        public event EventHandler<BluetoothDevice>? BeaconFound;

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

        private void ProcessScanResult(ScanResult result)
        {
            BeaconFound?.Invoke(this,result.Device);
        }
    }
}