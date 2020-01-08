using System.Threading.Tasks;

namespace Sanet.SmartSkating.Services.Api
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Debug.Mock", "1.0.0.0")]
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