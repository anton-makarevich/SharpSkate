using System.Threading.Tasks;
using Sanet.SmartSkating.Dto.Services;

namespace Sanet.SmartSkating.Services.Api
{
    public class DataSyncService:IDataSyncService
    {
        private readonly IDataService _dataService;
        private readonly IApiService _apiService;
        private readonly IConnectivityService _connectivityService;

        private bool _isStarted;

        public DataSyncService(IDataService dataService, IApiService apiService,
            IConnectivityService connectivityService)
        {
            _dataService = dataService;
            _apiService = apiService;
            _connectivityService = connectivityService;
        }

        public void StartSyncing()
        {
            if (_isStarted)
                return;
            _isStarted = true;
#pragma warning disable 4014
            SyncProcess();
#pragma warning restore 4014
        }

        private async Task SyncProcess()
        {
            do
            {
                if (!await _connectivityService.IsConnected())
                    continue;
                var wayPointsToSync = await _dataService.GetAllWayPointsAsync();
                var wayPointsIds = (await _apiService.PostWaypointsAsync(wayPointsToSync))
                    .SyncedWayPointsIds;
                foreach (var syncedWayPoint in wayPointsIds)
                {
                    await _dataService.DeleteWayPointAsync(syncedWayPoint);
                }
                await Task.Delay(30000);
            } while (true);
        }
    }
}