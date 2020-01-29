namespace Sanet.SmartSkating.Dto.Models
{
    public class BleDeviceDto:EntityBase
    {
        public string ParentId { get; set; } = string.Empty;
        public string DeviceName { get; set; } = string.Empty;
        public int WayPointType { get; set; } 
    }
}