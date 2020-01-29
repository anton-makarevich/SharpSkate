namespace Sanet.SmartSkating.Dto.Models
{
    public class DeviceDto:EntityBase
    {
        public string Manufacturer { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string OsName { get; set; } = string.Empty;
        public string OsVersion { get; set; } = string.Empty;
    }
}