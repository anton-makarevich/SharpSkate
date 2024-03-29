using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        private readonly IAccountService _accountService;
        private readonly IDataSyncService _dataSyncService;
        private readonly IBleLocationService _bleLocationService;
        private readonly ISettingsService _settingsService;
        private readonly ISessionProvider _sessionProvider;
        private readonly IApiService _apiService;
        private readonly ISyncService _syncService;
        private readonly IDateProvider _dateProvider;
        private readonly IConfigService _configService;

        public SessionManager(ILocationService locationService,
            IAccountService accountService,
            IDataSyncService dataSyncService,
            IBleLocationService bleLocationService,
            ISettingsService settingsService,
            ISessionProvider sessionProvider,
            IApiService apiService,
            ISyncService syncService, 
            IDateProvider dateProvider, 
            IConfigService configService)
        {
            _locationService = locationService;
            _accountService = accountService;
            _dataSyncService = dataSyncService;
            _bleLocationService = bleLocationService;
            _settingsService = settingsService;
            _sessionProvider = sessionProvider;
            _apiService = apiService;
            _syncService = syncService;
            _dateProvider = dateProvider;
            _configService = configService;
        }

        public ISession? CurrentSession => _sessionProvider.CurrentSession;
        public bool IsRunning { get; private set; }
        public bool IsCompleted { get; private set; }
        public bool CanStart =>IsReady
                               && !IsRunning
                               && !IsCompleted
                               && !IsRemote;

        public bool IsRemote => CurrentSession?.IsRemote == true;
        public event EventHandler? SessionUpdated;
        public bool IsReady => _sessionProvider.CurrentSession != null;

        public async ValueTask StartSession()
        {
            if (!CanStart)
                return;
            IsRunning = true;
            CurrentSession?.SetStartTime(_dateProvider.Now());
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

            IsRunning = !CurrentSession.IsCompleted;
#pragma warning disable 4014
            HandleRemoteSession();
#pragma warning restore 4014
        }

        private async Task HandleRemoteSession()
        {
            if (CurrentSession == null)
                return;
            var waypointsTask = _apiService.GetWaypointsForSessionAsync(
                            CurrentSession.SessionId,
                            _configService.AzureApiSubscriptionKey);
            var apiTasksForSession = new List<Task>
            {
                waypointsTask
            };
            if (IsRunning)
            {
                _syncService.WayPointReceived += SyncServiceOnWayPointReceived;
                _syncService.SessionClosedReceived += SyncServiceOnSessionClosedReceived;
                apiTasksForSession.Add(_syncService.ConnectToHub(CurrentSession.SessionId));
            }

            await Task.WhenAll(apiTasksForSession);
            var waypoints = (await waypointsTask).Waypoints;
            if (waypoints != null)
            {
                CurrentSession?.AddPoints(waypoints);
                SessionUpdated?.Invoke(this, null);
            }
        }

        private void SyncServiceOnSessionClosedReceived(object sender, SessionEventArgs e)
        {
            CloseRemoteSession();
        }

        private void CloseRemoteSession()
        {
            IsRunning = false;
            _syncService.WayPointReceived-= SyncServiceOnWayPointReceived; 
            _syncService.SessionClosedReceived-= SyncServiceOnSessionClosedReceived;
            _syncService.CloseConnection();
        }

        private void SyncServiceOnWayPointReceived(object sender, WayPointEventArgs waypointArgs)
        {
            CurrentSession?.AddPoint(
                new Coordinate(waypointArgs.WayPoint.Coordinate),
                waypointArgs.WayPoint.Time);
        }
        
        private void LocationServiceOnLocationReceived(object sender, CoordinateEventArgs e)
        {
            if (CurrentSession == null) return;
            var pointDto = WayPointDto.FromSessionCoordinate(
                CurrentSession.SessionId,
                _accountService.DeviceId,
                e.Coordinate.ToDto(),
                e.Date);
            _dataSyncService.SaveAndSyncWayPointAsync(pointDto);
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
                await _dataSyncService.SaveAndSyncSessionAsync(sessionDto);
            }
            
            await _dataSyncService.SyncWayPointsAsync();
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
