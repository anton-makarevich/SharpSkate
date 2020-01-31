using Microsoft.WindowsAzure.Storage.Table;
using Sanet.SmartSkating.Dto.Models;

namespace Sanet.SmartSkating.Backend.Azure.Models
{
    public class DeviceEntity:TableEntity
    {
        public DeviceEntity(DeviceDto deviceDto)
        {
            PartitionKey = $"{deviceDto.OsName}:{deviceDto.OsVersion}";
            RowKey = deviceDto.Id;

            DeviceInfo = $"{deviceDto.Manufacturer}-{deviceDto.Model}";
        }

        public string DeviceInfo { get; set; }
    }
}