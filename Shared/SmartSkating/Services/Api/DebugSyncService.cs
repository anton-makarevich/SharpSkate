using System.Threading.Tasks;

namespace Sanet.SmartSkating.Services.Api
{
    public class DebugSyncService:IDataSyncService
    {
        public void StartSyncing()
        {
        }

        public async Task SyncWayPointsAsync()
        {
            await Task.Delay(50);
        }

        public async Task SyncSessionsAsync()
        {
            await Task.Delay(50);
        }
    }
}