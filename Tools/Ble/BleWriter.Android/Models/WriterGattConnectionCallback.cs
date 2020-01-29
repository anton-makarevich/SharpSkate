using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Android.Bluetooth;
using Java.Util;

namespace BleWriter.Android.Models
{
    public class WriterGattConnectionCallback:BluetoothGattCallback
    {
        private readonly BluetoothDevice _device;

        public WriterGattConnectionCallback(BluetoothDevice device)
        {
            _device = device;
        }

        public override void OnConnectionStateChange(
            BluetoothGatt gatt, 
            GattStatus status,
            ProfileState newState)
        {
            base.OnConnectionStateChange(gatt, status, newState);
            if (status == GattStatus.Success)
            {
                switch (newState)
                {
                    case ProfileState.Connected:
                        gatt.DiscoverServices();
                        break;
                    case ProfileState.Disconnected:
                        gatt.Close();
                        break;
                }
            }
            else
            {
                gatt.Close();
            }
        }

        public override async void OnServicesDiscovered(BluetoothGatt gatt, GattStatus status)
        {
            if (status == GattStatus.Success)
            {
                await ReadAllCharacteristicsAsync(gatt);
            }
            else
            {
                gatt.Close();
            }
        }

        public async Task ReadAllCharacteristicsAsync(BluetoothGatt gatt)
        {
            var services = gatt.Services;
            foreach (var service in services)
            {
                foreach (var characteristic in service.Characteristics)
                {
                    if (characteristic.Properties.HasFlag(GattProperty.Read))
                    {
                        var value = await ReadCharacteristicAsync(gatt, characteristic);
                        if (value == _device.Name)
                        {
                            if (characteristic.Properties.HasFlag(GattProperty.Write))
                            {
                                var writeresult = await WriteNewNameAsync(gatt, characteristic);
                                var b = writeresult;
                            }
                        }
                    }
                }
            }
        }

        private Task<bool> WriteNewNameAsync(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic)
        {
            _writeTaskCompletionSource = new TaskCompletionSource<bool>();
            var bytesValue = Encoding.ASCII.GetBytes("AG");
            characteristic.SetValue(bytesValue);
            characteristic.WriteType = GattWriteType.Default;
            gatt.WriteCharacteristic(characteristic);
            return _writeTaskCompletionSource.Task;
        }

        private readonly Dictionary<UUID,TaskCompletionSource<string>> _readTaskSources 
            = new Dictionary<UUID, TaskCompletionSource<string>>();

        private TaskCompletionSource<bool>? _writeTaskCompletionSource;
        
        private Task<string> ReadCharacteristicAsync(BluetoothGatt gatt ,BluetoothGattCharacteristic characteristic)
        {
            var readCharacteristicTaskCompletion = new TaskCompletionSource<string>();
            if (_readTaskSources.ContainsKey(characteristic.Uuid))
            {
                _readTaskSources[characteristic.Uuid] = readCharacteristicTaskCompletion;
            }
            else
            {
                _readTaskSources.Add(characteristic.Uuid, readCharacteristicTaskCompletion);
            }
            gatt.ReadCharacteristic(characteristic);
            return readCharacteristicTaskCompletion.Task;
        }

        public override void OnCharacteristicRead(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic, GattStatus status)
        {
            if (!_readTaskSources.ContainsKey(characteristic.Uuid))
                return;
            var readCharacteristicTaskCompletion = _readTaskSources[characteristic.Uuid];
            readCharacteristicTaskCompletion.SetResult(status == GattStatus.Success
                ? characteristic.GetStringValue(0)
                : status.ToString());
            base.OnCharacteristicRead(gatt, characteristic, status);
        }

        public override void OnCharacteristicWrite(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic, GattStatus status)
        {
            _writeTaskCompletionSource?.SetResult(status == GattStatus.Success);
            base.OnCharacteristicWrite(gatt, characteristic, status);
        }
    }
}