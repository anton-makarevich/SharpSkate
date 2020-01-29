namespace Sanet.SmartSkating.Dto.Models
{
    public class TrackDto:EntityBase
    {
        public string Name { get; set; } = string.Empty;
        public CoordinateDto Start { get; set; }
        public CoordinateDto Finish { get; set; }
    }
}