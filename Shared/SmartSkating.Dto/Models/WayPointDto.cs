using System;

namespace Sanet.SmartSkating.Dto.Models
{
    public class WayPointDto:EntityBase
    {
        public string SessionId { get; set; }
        public CoordinateDto Coordinate { get; set; }
        public string WayPointType { get; set; }
        public DateTime Time { get; set; }

        public static WayPointDto FromSessionCoordinate(string sessionId, CoordinateDto coordinate)
        {
            return new WayPointDto()
            {
                Coordinate = coordinate,
                Id = Guid.NewGuid().ToString("N"),
                SessionId = sessionId,
                Time = DateTime.UtcNow,
                WayPointType = string.Empty
            };
        }
    }
}