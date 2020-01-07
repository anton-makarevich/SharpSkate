namespace Sanet.SmartSkating.Dto.Models
{
    public class TrackDto:EntityBase
    {
        public string Name { get; set; }
        public CoordinateDto Start { get; set; }
        public CoordinateDto Finish { get; set; }
    }
}