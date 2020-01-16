using System.Threading.Tasks;
using BleWriter.Services;
using Sanet.SmartSkating.Dto.Models;

namespace BleWriter.Android.Services
{
    public class AndroidBleWriter:IBleWriterService
    {
        public Task WriteDeviceIdAsync(BleDeviceDto deviceStub)
        {
            throw new System.NotImplementedException();
        }
    }
}