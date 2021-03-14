using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sanet.SmartSkating.Dto;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Dto.Services;
using Sanet.SmartSkating.Models.EventArgs;
using Sanet.SmartSkating.Models.Location;
using Sanet.SmartSkating.Models.Training;
using Sanet.SmartSkating.Services.Account;
using Sanet.SmartSkating.Services.Api;
using Sanet.SmartSkating.Services.Location;

namespace Sanet.SmartSkating.Services.Tracking
{
    public class SessionManager:ISessionManager
    {
        private readonly ILocationService _locationService;
        private readonly IDataService _storageService;
        private readonly IAccountService _accountService;
        private readonly IDataSyncService _dataSyncService;
        private readonly IBleLocationService _bleLocationService;
        private readonly ISettingsService _settingsService;
        private readonly ISessionProvider _sessionProvider;
        private readonly IApiService _apiService;
        private readonly ISyncService _syncService;

        public SessionManager(ILocationService locationService,
            IDataService storageService,
            IAccountService accountService,
            IDataSyncService dataSyncService,
            IBleLocationService bleLocationService,
            ISettingsService settingsService,
            ISessionProvider sessionProvider,
            IApiService apiService,
            ISyncService syncService)
        {
            _locationService = locationService;
            _storageService = storageService;
            _accountService = accountService;
            _dataSyncService = dataSyncService;
            _bleLocationService = bleLocationService;
            _settingsService = settingsService;
            _sessionProvider = sessionProvider;
            _apiService = apiService;
            _syncService = syncService;
        }

        public ISession? CurrentSession => _sessionProvider.CurrentSession;
        public bool IsRunning { get; private set; }
        public bool IsCompleted { get; private set; }
        public bool IsReady => _sessionProvider.CurrentSession != null;

        public async ValueTask StartSession()
        {
            if (!IsReady || CurrentSession?.IsRemote == true)
                return;
            IsRunning = true;
            if (_settingsService.UseBle)
                await _bleLocationService.LoadDevicesDataAsync();

            _locationService.LocationReceived+= LocationServiceOnLocationReceived;
            _locationService.StartFetchLocation();

            if (_settingsService.UseBle)
            {
                _bleLocationService.CheckPointPassed+= BleLocationServiceOnCheckPointPassed;
                _bleLocationService.StartBleScan(CurrentSession?.SessionId??string.Empty);
            }
#pragma warning disable 4014
            SaveSessionAndSyncData();
#pragma warning restore 4014
        }

        public void StopSession()
        {
            if (CurrentSession?.IsRemote == true)
            {
                return;
            }
            StopLocationService();
            IsRunning = false;
#pragma warning disable 4014
            SaveSessionAndSyncData(true);
#pragma warning restore 4014
        }

        public void CheckSession()
        {
            if (CurrentSession?.IsRemote != true)
            {
                return;
            }

            IsRunning = true;
#pragma warning disable 4014
            HandleRemoteSession();
#pragma warning restore 4014
        }

        private async Task HandleRemoteSession()
        {
            if (CurrentSession != null)
            {
                var syncHubTask = _syncService.ConnectToHub(CurrentSession.SessionId);
                var waypointsTask = _apiService.GetWaypointsForSessionAsync(
                    CurrentSession.SessionId,
                    ApiNames.AzureApiSubscriptionKey);
                var apiTasksForSession = new[]
                {
                    syncHubTask,
                    waypointsTask
                };

                 await Task.WhenAll(apiTasksForSession);
                 var waypoints = (await waypointsTask).Waypoints;
                 AddWaypointsToSession(waypoints);
            }
        }
        
        private void AddWaypointsToSession(IReadOnlyCollection<WayPointDto>? waypoints)
        {
            if (waypoints == null) return;
            foreach (var waypoint in waypoints)
            {
                CurrentSession?.AddPoint(new Coordinate(waypoint.Coordinate), waypoint.Time);
            }
        }

        private void LocationServiceOnLocationReceived(object sender, CoordinateEventArgs e)
        {
            if (CurrentSession == null) return;
            var pointDto = WayPointDto.FromSessionCoordinate(
                CurrentSession.SessionId,
                _accountService.DeviceId,
                e.Coordinate.ToDto(),
                e.Date);
            _storageService.SaveWayPointAsync(pointDto);
            CurrentSession?.AddPoint(e.Coordinate,pointDto.Time);
        }

        private void BleLocationServiceOnCheckPointPassed(object sender, CheckPointEventArgs e)
        {
            CurrentSession?.AddSeparatingPoint(e.WayPointType,e.Date??DateTime.UtcNow);
        }

        private async Task SaveSessionAndSyncData(bool isCompleted = false)
        {
            IsCompleted = isCompleted;
            var sessionDto = GetSessionDto();
            if (sessionDto != null)
            {
                sessionDto.IsCompleted = isCompleted;
                await _storageService.SaveSessionAsync(sessionDto);
            }

            if (isCompleted)
            {
                await _dataSyncService.SyncSessionsAsync();
                await _dataSyncService.SyncWayPointsAsync();
            }
        }

        private SessionDto? GetSessionDto()
        {
            if (CurrentSession == null)
                return null;
            var s = new SessionDto
            {
                Id = CurrentSession.SessionId,
                AccountId = _accountService.UserId,
                DeviceId = _accountService.DeviceId,
                RinkId = CurrentSession.Rink.Id,
                StartTime = CurrentSession.StartTime
            };
            return s;
        }

        private void StopLocationService()
        {
            _locationService.LocationReceived -= LocationServiceOnLocationReceived;
            _bleLocationService.CheckPointPassed-= BleLocationServiceOnCheckPointPassed;

            _locationService.StopFetchLocation();
            _bleLocationService.StopBleScan();

#pragma warning disable 4014
            SaveSessionAndSyncData(true);
#pragma warning restore 4014
        }
    }
}
