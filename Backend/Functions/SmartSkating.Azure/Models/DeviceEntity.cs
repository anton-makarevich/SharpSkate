using Microsoft.WindowsAzure.Storage.Table;
using Sanet.SmartSkating.Dto.Models;

namespace Sanet.SmartSkating.Backend.Azure.Models
{
    public class DeviceEntity:TableEntity
    {
        public DeviceEntity(DeviceDto deviceDto)
        {
            PartitionKey = deviceDto.AccountId;
            RowKey = deviceDto.Id;
            OsInfo = $"{deviceDto.OsName}:{deviceDto.OsVersion}";
            DeviceInfo = $"{deviceDto.Manufacturer}-{deviceDto.Model}";
        }

        public string DeviceInfo { get; set; }
        public string OsInfo { get; set; }
    }
}