using System;

namespace Sanet.SmartSkating.Dto.Models
{
    public class WayPointDto:EntityBase
    {
        public string SessionId { get; set; } = "";
        public CoordinateDto Coordinate { get; set; }
        public DateTime Time { get; set; }
        public string DeviceId { get; set; } = "";

        public static WayPointDto FromSessionCoordinate(string sessionId, string deviceId, CoordinateDto coordinate, DateTime? time = null)
        {
            return new WayPointDto
            {
                Coordinate = coordinate,
                Id = Guid.NewGuid().ToString("N"),
                SessionId = sessionId,
                DeviceId = deviceId,
                Time = time?? DateTime.UtcNow
            };
        }
    }
}
