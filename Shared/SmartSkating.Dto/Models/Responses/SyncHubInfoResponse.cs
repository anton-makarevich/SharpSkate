using Sanet.SmartSkating.Dto.Models.Responses.Base;

namespace Sanet.SmartSkating.Dto.Models.Responses
{
    public class SyncHubInfoResponse:ResponseBase
    {
        public SyncHubInfoDto? SyncHubInfo { get; set; }
    }
}
