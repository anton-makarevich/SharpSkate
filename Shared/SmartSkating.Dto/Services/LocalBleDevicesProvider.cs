using System.Collections.Generic;
using System.Threading.Tasks;
using Sanet.SmartSkating.Dto.Models;

namespace Sanet.SmartSkating.Dto.Services
{
    public class LocalBleDevicesProvider : IBleDevicesProvider
    {
        private readonly IResourceReader _resourceReader;

        public LocalBleDevicesProvider(IResourceReader resourceReader)
        {
            _resourceReader = resourceReader;
        }
        public Task<List<BleDeviceDto>> GetBleDevicesAsync()
        {
            return _resourceReader.ReadEmbeddedResourceAsync<LocalBleDevicesProvider, BleDeviceDto>("ble.json");
        }
    }
}