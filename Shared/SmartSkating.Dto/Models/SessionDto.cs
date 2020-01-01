using System;

namespace Sanet.SmartSkating.Dto.Models
{
    public class SessionDto:EntityBase
    {
        public string AccountId { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public bool IsSaved { get; set; }
    }
}