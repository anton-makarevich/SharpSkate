using System.Threading.Tasks;
using Sanet.SmartSkating.Dto.Models;

namespace Sanet.SmartSkating.Services.Api
{
    public interface IDataSyncService
    {
        void StartSyncing();
        Task SyncWayPointsAsync();
        Task SyncSessionsAsync();
        Task SyncBleScansAsync();
        Task SaveAndSyncSessionAsync(SessionDto sessionDto);
        Task SaveAndSyncWayPointAsync(WayPointDto pointDto);
    }
}   