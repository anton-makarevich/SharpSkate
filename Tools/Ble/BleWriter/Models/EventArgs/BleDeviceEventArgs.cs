using Sanet.SmartSkating.Dto.Models;

namespace Sanet.SmartSkating.Models.EventArgs
{
    public class BleDeviceEventArgs:System.EventArgs
    {
        public BleDeviceDto BleDevice { get; }

        public BleDeviceEventArgs(BleDeviceDto bleDevice)
        {
            BleDevice = bleDevice;
        }
    }
}