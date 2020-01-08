namespace Sanet.SmartSkating.Dto.Models
{
    public class BleDeviceDto:EntityBase
    {
        public string ParentId { get; set; }
        public string DeviceName { get; set; }
        public int WayPointType { get; set; } 
    }
}