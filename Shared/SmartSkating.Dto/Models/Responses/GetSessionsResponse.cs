using System.Collections.Generic;
using Sanet.SmartSkating.Dto.Models.Responses.Base;

namespace Sanet.SmartSkating.Dto.Models.Responses
{
    public class GetSessionsResponse:ResponseBase
    {
        public List<SessionDto> Sessions { get; set; }
    }
}