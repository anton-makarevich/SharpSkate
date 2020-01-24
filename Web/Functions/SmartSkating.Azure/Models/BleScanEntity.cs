using System;
using Microsoft.WindowsAzure.Storage.Table;
using Sanet.SmartSkating.Dto.Models;

namespace Sanet.SmartSkating.Azure.Models
{
    public class BleScanEntity:TableEntity
    {
        public BleScanEntity(BleScanResultDto bleScan)
        {
            PartitionKey = bleScan.SessionId;
            RowKey = bleScan.Id;

            DeviceId = bleScan.DeviceAddress;
            Rssi = bleScan.Rssi;
  
            Time = bleScan.Time;
        }

        public string DeviceId { get; set; }

        public int Rssi { get; set; }

        public DateTime Time { get; set; }
    }
}