using Microsoft.WindowsAzure.Storage.Table;
using Sanet.SmartSkating.Dto.Models;

namespace Sanet.SmartSkating.Backend.Azure.Models
{
    public class SessionEntity:TableEntity
    {
        public SessionEntity()
        {
        }
        public SessionEntity(SessionDto sessionDto)
        {
            PartitionKey = sessionDto.AccountId;
            RowKey = sessionDto.Id;
            IsCompleted = sessionDto.IsCompleted;
            DeviceId = sessionDto.DeviceId;
            RinkId= sessionDto.RinkId;
        }

        public string RinkId { get; set; }

        public string DeviceId { get; set; }

        public bool IsCompleted { get; set; }

        public SessionDto ToDto()
        {
            return new SessionDto
            {
                Id = RowKey,
                AccountId = PartitionKey,
                IsCompleted = IsCompleted,
                IsSaved = true,
                DeviceId = DeviceId,
                RinkId = RinkId
            };
        }
    }
}
