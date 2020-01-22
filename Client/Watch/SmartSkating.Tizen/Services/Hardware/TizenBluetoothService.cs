using System.Threading.Tasks;
using Sanet.SmartSkating.Services.Hardware;
using Tizen.Applications;
using Tizen.Network.Bluetooth;

namespace Sanet.SmartSkating.Tizen.Services.Hardware
{
    public class TizenBluetoothService:IBluetoothService
    {
        public bool IsBluetoothAvailable()
        {
            return BluetoothAdapter.IsBluetoothEnabled;
        }

        public Task EnableBluetoothAsync()
        {
            return Task.Run(() =>
            {
                if (IsBluetoothAvailable()) return;
                var myAppControl = new AppControl
                {
                    Operation = AppControlOperations.SettingBluetoothEnable
                };
                AppControl.SendLaunchRequest(myAppControl);
            });
        }
    }
}