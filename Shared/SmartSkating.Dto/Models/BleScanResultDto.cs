using System;

namespace Sanet.SmartSkating.Dto.Models
{
    public class BleScanResultDto:EntityBase
    {
        public string DeviceAddress { get; set; } = string.Empty;
        public int Rssi { get; set; }
        public DateTime Time { get; set; }
        public string SessionId { get; set; } = string.Empty;
    }
}