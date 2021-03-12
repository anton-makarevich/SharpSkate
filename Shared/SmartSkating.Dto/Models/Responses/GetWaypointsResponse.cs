using System.Collections.Generic;
using Sanet.SmartSkating.Dto.Models.Responses.Base;

namespace Sanet.SmartSkating.Dto.Models.Responses
{
    public class GetWaypointsResponse:ResponseBase
    {
        public List<WayPointDto>? Waypoints { get; set; }
    }
}