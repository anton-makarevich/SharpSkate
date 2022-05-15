using System.Threading.Tasks;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Services.Api;

namespace Sanet.SmartSkating.Dashboard.Services.Dummy
{
    public class DummyDataSyncService:IDataSyncService
    {
        public void StartSyncing()
        {
        }

        public async Task SyncWayPointsAsync()
        {
        }

        public async Task SyncSessionsAsync()
        {
        }

        public async Task SyncBleScansAsync()
        {
        }

        public async Task SaveAndSyncSessionAsync(SessionDto sessionDto)
        {
        }

        public async Task SaveAndSyncWayPointAsync(WayPointDto pointDto)
        {
        }
    }
}