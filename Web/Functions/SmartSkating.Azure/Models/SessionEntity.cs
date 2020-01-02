using Microsoft.WindowsAzure.Storage.Table;
using Sanet.SmartSkating.Dto.Models;

namespace Sanet.SmartSkating.Azure.Models
{
    public class SessionEntity:TableEntity
    {
        public SessionEntity(SessionDto sessionDto)
        {
            PartitionKey = sessionDto.AccountId;
            RowKey = sessionDto.Id;
            IsCompleted = sessionDto.IsCompleted;
        }

        public bool IsCompleted { get; set; }
    }
}