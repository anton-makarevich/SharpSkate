namespace Sanet.SmartSkating.Dto.Models
{
    public struct WayPointDto
    {
        public string Id { get; set; }
        public string SessionId { get; set; }
        public CoordinateDto Coordinate { get; set; }
        public string WayPointType { get; set; }
    }
}