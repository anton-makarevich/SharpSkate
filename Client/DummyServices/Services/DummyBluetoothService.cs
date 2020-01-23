using System.Threading.Tasks;
using Sanet.SmartSkating.Services.Hardware;

namespace Sanet.SmartSkating.Xf.Droid.Services
{
    public class DummyBluetoothService: IBluetoothService
    {
        public bool IsBluetoothAvailable()
        {
            return true;
        }

        public Task EnableBluetoothAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}