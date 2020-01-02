using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using Sanet.SmartSkating.Dto;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Dto.Models.Responses;

namespace Sanet.SmartSkating.Services.Api
{
    public interface IApiService
    {
        [Post(ApiNames.WayPointsResource.Path)]
        Task<SaveEntitiesResponse> PostWaypointsAsync([Body] List<WayPointDto> waypoints);

        [Post(ApiNames.SessionsResource.Path)]
        Task<SaveEntitiesResponse> PostSessionsAsync([Body] List<SessionDto> sessions);
    }
}