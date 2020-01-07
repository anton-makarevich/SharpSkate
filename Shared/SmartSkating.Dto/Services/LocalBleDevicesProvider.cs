using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sanet.SmartSkating.Dto.Models;

namespace Sanet.SmartSkating.Dto.Services
{
    public class LocalBleDevicesProvider : IBleDevicesProvider
    {
        public Task<List<BleDeviceDto>> GetBleDevicesAsync()
        {
            return Task.Run(() =>
            {
                var assembly = Assembly.GetAssembly(typeof(LocalBleDevicesProvider));
                var resourceName = assembly.GetManifestResourceNames()
                    .FirstOrDefault(f=> f.ToLower().EndsWith("ble.json"));
                using var stream = assembly.GetManifestResourceStream(resourceName);
                using var reader = new StreamReader(stream 
                                                    ?? throw new MissingManifestResourceException(
                                                        $"Missing local ble devices file"));
                var result = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<List<BleDeviceDto>>(result);
            }); 
        }
    }
}