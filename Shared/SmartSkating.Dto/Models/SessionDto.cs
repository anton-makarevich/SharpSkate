namespace Sanet.SmartSkating.Dto.Models
{
    public class SessionDto:EntityBase
    {
        public string AccountId { get; set; } = "";
        public bool IsCompleted { get; set; }
        public bool IsSaved { get; set; }
        public string RinkId { get; set; } = "";
        public string DeviceId { get; set; } = "";
    }
}
