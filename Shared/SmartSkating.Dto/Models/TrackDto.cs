namespace Sanet.SmartSkating.Dto.Models
{
    public struct TrackDto
    {
        public string Name { get; set; }
        public CoordinateDto Start { get; set; }
        public CoordinateDto Finish { get; set; }
    }
}