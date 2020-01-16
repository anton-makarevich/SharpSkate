using System.Threading.Tasks;
using Sanet.SmartSkating.Dto.Models;

namespace BleWriter.Services
{
    public interface IBleWriterService
    {
        Task WriteDeviceIdAsync(BleDeviceDto deviceStub);
    }
}