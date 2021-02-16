using System;
using System.Threading.Tasks;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Dto.Services;
using Sanet.SmartSkating.Models.EventArgs;
using Sanet.SmartSkating.Models.Geometry;
using Sanet.SmartSkating.Models.Training;
using Sanet.SmartSkating.Services.Account;
using Sanet.SmartSkating.Services.Api;
using Sanet.SmartSkating.Services.Location;

namespace Sanet.SmartSkating.Services.Tracking
{
    public class SessionService:ISessionService
    {
        private readonly ILocationService _locationService;
        private readonly IDataService _storageService;
        private readonly ITrackService _trackService;
        private readonly IAccountService _accountService;
        private readonly IDataSyncService _dataSyncService;
        private readonly IBleLocationService _bleLocationService;
        private readonly ISettingsService _settingsService;

        public SessionService(ILocationService locationService,
            IDataService storageService,
            ITrackService trackService,
            IAccountService accountService,
            IDataSyncService dataSyncService,
            IBleLocationService bleLocationService,
            ISettingsService settingsService)
        {
            _locationService = locationService;
            _storageService = storageService;
            _trackService = trackService;
            _accountService = accountService;
            _dataSyncService = dataSyncService;
            _bleLocationService = bleLocationService;
            _settingsService = settingsService;
        }
        public ISession CreateSessionForRink(Rink rink)
        {
            CurrentSession = new Session(rink,_settingsService);
            return CurrentSession;
        }

        public ISession? CurrentSession { get; private set; }
        public async Task StartSession()
        {
            if (_settingsService.UseBle)
                await _bleLocationService.LoadDevicesDataAsync();

            _locationService.LocationReceived+= LocationServiceOnLocationReceived;
            _locationService.StartFetchLocation();

            if (_settingsService.UseBle)
            {
                _bleLocationService.CheckPointPassed+= BleLocationServiceOnCheckPointPassed;
                _bleLocationService.StartBleScan(CurrentSession?.SessionId??string.Empty);
            }
            SaveSessionAndSyncData();
        }

        public void StopSession()
        {
            StopLocationService();
        }

        private void LocationServiceOnLocationReceived(object sender, CoordinateEventArgs e)
        {
            if (CurrentSession != null)
            {
                var pointDto = WayPointDto.FromSessionCoordinate(
                    CurrentSession.SessionId,
                    _accountService.DeviceId,
                    e.Coordinate.ToDto(),
                    e.Date);
                _storageService.SaveWayPointAsync(pointDto);
                CurrentSession?.AddPoint(e.Coordinate,pointDto.Time);
            }

            //UpdateMetaData();
        }

        private void BleLocationServiceOnCheckPointPassed(object sender, CheckPointEventArgs e)
        {
            CurrentSession?.AddSeparatingPoint(e.WayPointType,e.Date??DateTime.UtcNow);
            //UpdateMetaData();
        }

        private async Task SaveSessionAndSyncData(bool isCompleted = false)
        {
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
                DeviceId = _accountService.GetDeviceInfo().Id,
                RinkId = _trackService.SelectedRink?.Id??""
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
