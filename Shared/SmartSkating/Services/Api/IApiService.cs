using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Dto.Models.Responses;

namespace Sanet.SmartSkating.Services.Api
{
    public interface IApiService
    {
        [Post("/waypoints")]
        Task<SaveEntitiesResponse> PostWaypointsAsync(
            [Body] List<WayPointDto> waypoints,
            [Header("Ocp-Apim-Subscription-Key")] string subscriptionKey
            );

        [Post("/sessions")]
        Task<SaveEntitiesResponse> PostSessionsAsync(
            [Body] List<SessionDto> sessions,
            [Header("Ocp-Apim-Subscription-Key")] string subscriptionKey
        );
        
        [Post("/scans")]
        Task<SaveEntitiesResponse> PostBleScansAsync(
            [Body] List<BleScanResultDto> scans,
            [Header("Ocp-Apim-Subscription-Key")] string subscriptionKey
        );
    }
}