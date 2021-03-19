using System.Threading.Tasks;
using Sanet.SmartSkating.Dto.Models;

namespace Sanet.SmartSkating.Services.Api
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Debug.Mock", "1.0.0.0")]
    public class DebugSyncService:IDataSyncService
    {
        public void StartSyncing()
        {
        }

        public Task SyncWayPointsAsync()
        {
            return Task.Delay(50);
        }

        public Task SyncSessionsAsync()
        {
            return Task.Delay(50);
        }

        public Task SyncBleScansAsync()
        {
            return Task.Delay(50);
        }

        public Task SaveAndSyncSessionAsync(SessionDto sessionDto)
        {
            return Task.Delay(50);
        }

        public Task SaveAndSyncWayPointAsync(WayPointDto pointDto)
        {
            return Task.Delay(50);
        }
    }
}