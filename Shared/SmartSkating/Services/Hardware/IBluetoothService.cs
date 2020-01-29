using System.Threading.Tasks;

namespace Sanet.SmartSkating.Services.Hardware
{
    public interface IBluetoothService
    {
        bool IsBluetoothAvailable();

        Task EnableBluetoothAsync();
    }
}