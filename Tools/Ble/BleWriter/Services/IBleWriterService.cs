using System;
using System.Threading.Tasks;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Models.EventArgs;

namespace BleWriter.Services
{
    public interface IBleWriterService
    {
        Task WriteDeviceIdAsync(BleDeviceDto deviceStub);
        void StopBleScan();
        event EventHandler<BleDeviceEventArgs>? NewBleDeviceFound;
        void StartBleScan();
    }
}