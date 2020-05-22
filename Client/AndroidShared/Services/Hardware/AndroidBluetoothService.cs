using System.Threading.Tasks;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Sanet.SmartSkating.Services.Hardware;

namespace Sanet.SmartSkating.Xf.Droid.AndroidShared.Services.Hardware
{
    public class AndroidBluetoothService:IBluetoothService
    {
        private readonly Context _context;

        public AndroidBluetoothService(Context context)
        {
            _context = context;
        }

        public bool IsBluetoothAvailable()
        {
            try
            {
                return BluetoothAdapter.DefaultAdapter.IsEnabled;
            }
            catch
            {
                return false;
            }
        }

        public Task EnableBluetoothAsync()
        {
            return Task.Run(() =>
            {
                if (IsBluetoothAvailable()) return;
                var intent = new Intent(BluetoothAdapter.ActionRequestEnable);
                _context.StartActivity(intent, Bundle.Empty);
            });
        }
    }
}