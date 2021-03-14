using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Dto.Models.Requests;
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

        [Post("/devices")]
        Task<BooleanResponse> PostDeviceAsync(
            [Body] DeviceDto device, 
            [Header("Ocp-Apim-Subscription-Key")] string subscriptionKey);

        [Post("/accounts")]
        Task<LoginResponse> LoginAsync(
            [Body] LoginRequest request, 
            [Header("Ocp-Apim-Subscription-Key")] string subscriptionKey);

        [Get("/sessions")]
        Task<GetSessionsResponse> GetSessionsAsync(
            [Query] string accountId,
            [Query] bool activeOnly,
            [Header("Ocp-Apim-Subscription-Key")] string subscriptionKey);

        [Get("/waypoints")]
        Task<GetWaypointsResponse> GetWaypointsForSessionAsync(
            [Query] string sessionId,
            [Header("Ocp-Apim-Subscription-Key")] string subscriptionKey);
    }
}