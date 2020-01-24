using System.Threading.Tasks;

namespace Sanet.SmartSkating.Services.Api
{
    public interface IDataSyncService
    {
        void StartSyncing();
        Task SyncWayPointsAsync();
        Task SyncSessionsAsync();
        Task SyncBleScansAsync();
    }
}