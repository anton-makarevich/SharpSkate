using System;

namespace Sanet.SmartSkating.Dto.Models
{
    public class WayPointDto:EntityBase
    {
        public string SessionId { get; set; } = string.Empty;
        public CoordinateDto Coordinate { get; set; }
        public DateTime Time { get; set; }

        public static WayPointDto FromSessionCoordinate(string sessionId, CoordinateDto coordinate, DateTime? time = null)
        {
            return new WayPointDto
            {
                Coordinate = coordinate,
                Id = Guid.NewGuid().ToString("N"),
                SessionId = sessionId,
                Time = time?? DateTime.UtcNow
            };
        }
    }
}
