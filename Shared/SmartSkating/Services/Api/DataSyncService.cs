using System.Linq;
using System.Threading.Tasks;
using Sanet.SmartSkating.Dto;
using Sanet.SmartSkating.Dto.Services;
using Sanet.SmartSkating.Services.Account;

namespace Sanet.SmartSkating.Services.Api
{
    public class DataSyncService:IDataSyncService
    {
        private readonly IDataService _dataService;
        private readonly IApiService _apiService;
        private readonly IConnectivityService _connectivityService;
        private readonly IAccountService _accountService;

        private bool _isStarted;

        public DataSyncService(IDataService dataService, IApiService apiService,
            IConnectivityService connectivityService, IAccountService accountService)
        {
            _dataService = dataService;
            _apiService = apiService;
            _connectivityService = connectivityService;
            _accountService = accountService;
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
                await SyncDeviceAsync();
                await SyncSessionsAsync();
                await SyncWayPointsAsync();
                await SyncBleScansAsync();
            
                await Task.Delay(30000);
            } while (true);
            // ReSharper disable once FunctionNeverReturns
        }

        private async Task SyncDeviceAsync()
        {
            if (!await _connectivityService.IsConnected())
                return;
            var device = _accountService.GetDeviceInfo();
            await _apiService.PostDeviceAsync(device, ApiNames.AzureApiSubscriptionKey);
        }

        public async Task SyncWayPointsAsync()
        {
            if (!await _connectivityService.IsConnected())
                return;
            var wayPointsToSync = await _dataService.GetAllWayPointsAsync();
            if (wayPointsToSync.Count==0)
                return;
            var wayPointsIds = (await _apiService.PostWaypointsAsync(
                    wayPointsToSync, 
                    ApiNames.AzureApiSubscriptionKey))
                .SyncedIds;
            if (wayPointsIds == null) return;
            foreach (var syncedWayPoint in wayPointsIds)
            {
                await _dataService.DeleteWayPointAsync(syncedWayPoint);
            }
        }
        
        public async Task SyncSessionsAsync()
        {
            if (!await _connectivityService.IsConnected())
                return;
            var sessionsToSync = (await _dataService.GetAllSessionsAsync())
                .Where(s=>!s.IsSaved||s.IsCompleted)
                .ToList();
            if (sessionsToSync.Count==0)
                return;
            var syncedIds = (await _apiService.PostSessionsAsync(
                    sessionsToSync, 
                    ApiNames.AzureApiSubscriptionKey))
                .SyncedIds;
            if (syncedIds == null) return;
            foreach (var id in syncedIds)
            {
                var session = sessionsToSync.Single(s => s.Id == id);
                if (!session.IsSaved && !session.IsCompleted)
                {
                    session.IsSaved = true;
                    await _dataService.SaveSessionAsync(session);   
                }
                if (session.IsCompleted)
                    await _dataService.DeleteSessionAsync(id);
            }
        }

        public async Task SyncBleScansAsync()
        {
            if (!await _connectivityService.IsConnected())
                return;
            var bleScansToSync = await _dataService.GetAllBleScansAsync();
            if (bleScansToSync.Count==0)
                return;
            var syncedIds = (await _apiService.PostBleScansAsync(
                    bleScansToSync, 
                    ApiNames.AzureApiSubscriptionKey))
                .SyncedIds;
            if (syncedIds == null) return;
            foreach (var syncedScan in syncedIds)
            {
                await _dataService.DeleteBleScanAsync(syncedScan);
            }
        }
    }
}