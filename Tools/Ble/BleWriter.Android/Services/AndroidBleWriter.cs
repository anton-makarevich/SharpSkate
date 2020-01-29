using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.Bluetooth;
using Android.Bluetooth.LE;
using Android.Content;
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

        private readonly Context _context;

        public AndroidBleWriter(Context context)
        {
            _context = context;
        }

        private const string DeviceName = "RDL51822";
        
        public Task WriteDeviceIdAsync(BleDeviceDto deviceStub)
        {
            return Task.Run(() =>
            {
                foreach (var device in _knownDevices)
                {
                    var gatt = device.ConnectGatt(_context,
                        false,
                        new WriterGattConnectionCallback(device), 
                        BluetoothTransports.Le
                        );
                    var b = gatt;
                }
            });
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
            _callBack.BeaconFound += CallBackOnBeaconFound;

            var filter = new ScanFilter.Builder()
                    .SetDeviceName(DeviceName)
                    .Build();

            var settings = new ScanSettings.Builder()
                .SetScanMode(ScanMode.LowPower)
                .SetMatchMode(BluetoothScanMatchMode.Aggressive)
                .SetNumOfMatches(1)
                .Build();
            _bleScanner.StartScan(
                new List<ScanFilter> {filter},
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