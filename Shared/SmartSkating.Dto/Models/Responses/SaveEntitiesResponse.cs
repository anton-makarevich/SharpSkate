using System.Collections.Generic;
using Sanet.SmartSkating.Dto.Models.Responses.Base;

namespace Sanet.SmartSkating.Dto.Models.Responses
{
    public class SaveEntitiesResponse:ResponseBase
    {
        public List<string>? SyncedIds { get; set; } 
    }
}