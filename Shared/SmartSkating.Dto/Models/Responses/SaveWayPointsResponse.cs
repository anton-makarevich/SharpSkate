using System.Collections.Generic;
using Sanet.SmartSkating.Dto.Models.Responses.Base;

namespace Sanet.SmartSkating.Dto.Models.Responses
{
    public class SaveWayPointsResponse:ResponseBase
    {
        public List<string>? SyncedWayPointsIds { get; set; } 
    }
}