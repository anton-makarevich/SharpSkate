using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Dto.Services;
using Sanet.SmartSkating.Models;
using Sanet.SmartSkating.Models.EventArgs;
using Sanet.SmartSkating.Models.Location;
using Sanet.SmartSkating.Models.Training;
using Sanet.SmartSkating.Services;
using Sanet.SmartSkating.Services.Account;
using Sanet.SmartSkating.Services.Api;
using Sanet.SmartSkating.Services.Location;
using Sanet.SmartSkating.Services.Tracking;
using Sanet.SmartSkating.ViewModels.Base;

namespace Sanet.SmartSkating.ViewModels
{
    public class LiveSessionViewModel:BaseViewModel
    {
        public const string NoValue = "- - -";
        public const string TotalTimeFormat = "h\\:mm\\:ss";
        
        private readonly ILocationService _locationService;
        private readonly IDataService _storageService;
        private readonly ITrackService _trackService;
        private readonly ISessionService _sessionService;
        private ISession? _currentSession;
        private bool _isRunning;
        private string _infoLabel = string.Empty;
        private string _currentSector = NoValue;
        private string _lastLapTime = string.Empty;
        private string _laps = string.Empty;
        private string _lastSectorTime = string.Empty;
        private string _distance = string.Empty;
        private string _totalTime = string.Empty;
        private string _bestLapTime = string.Empty;
        private string _bestSectorTime = string.Empty;
        private readonly IAccountService _accountService;
        private readonly IDataSyncService _dataSyncService;
        private readonly IBleLocationService _bleLocationService;
        private readonly ISettingsService _settingsService;

        public LiveSessionViewModel(ILocationService locationService,
            IDataService storageService,
            ITrackService trackService, ISessionService sessionService, IAccountService accountService,
            IDataSyncService dataSyncService, IBleLocationService bleLocationService, ISettingsService settingsService)
        {
            _locationService = locationService;
            _storageService = storageService;
            _trackService = trackService;
            _sessionService = sessionService;
            _accountService = accountService;
            _dataSyncService = dataSyncService;
            _bleLocationService = bleLocationService;
            _settingsService = settingsService;

            TotalTime = new TimeSpan().ToString(TotalTimeFormat);
        }
        
        public ICommand StartCommand => new SimpleCommand(async() =>
        {
            if (_settingsService.UseBle)
                await _bleLocationService.LoadDevicesDataAsync();
            
            _locationService.LocationReceived+= LocationServiceOnLocationReceived;
            _locationService.StartFetchLocation();
            
            if (_settingsService.UseBle)
            {
                _bleLocationService.CheckPointPassed+= BleLocationServiceOnCheckPointPassed;
                _bleLocationService.StartBleScan(Session?.SessionId??string.Empty);
            }
            
            Session?.SetStartTime(DateTime.UtcNow);
            IsRunning = true;
#pragma warning disable 4014
            TrackTime();
            SaveSessionAndSyncData();
#pragma warning restore 4014
        });

        private void BleLocationServiceOnCheckPointPassed(object sender, CheckPointEventArgs e)
        {
            Session?.AddSeparatingPoint(e.WayPointType,e.Date??DateTime.UtcNow);
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

        private async Task TrackTime()
        {
            do
            {
                await Task.Delay(1000);
                if (Session == null) continue;
                var time = DateTime.UtcNow.Subtract(Session.StartTime);
                TotalTime = time.ToString(TotalTimeFormat);
            } while (IsRunning);
        }

        public string TotalTime
        {
            get => _totalTime;
            private set => SetProperty(ref _totalTime, value);
        }

        private void LocationServiceOnLocationReceived(object sender, CoordinateEventArgs e)
        {
            LastCoordinate = e.Coordinate;
            if (Session != null)
            {
                var pointDto = WayPointDto.FromSessionCoordinate(
                    Session.SessionId,
                    LastCoordinate.ToDto(),
                    e.Date);
                _storageService.SaveWayPointAsync(pointDto);
                Session?.AddPoint(LastCoordinate,pointDto.Time);
            }

            UpdateMetaData();
        }

        private void UpdateMetaData()
        {
            InfoLabel = LastCoordinate.ToString();
            if (Session == null) return;
            if (Session.LapsCount > 0)
            {
                LastLapTime = Session.LastLapTime.ToString(TotalTimeFormat);
                BestLapTime = Session.BestLapTime.ToString(TotalTimeFormat);
            }
            else
            {
                LastLapTime = NoValue;
                BestLapTime = NoValue;
            }

            Laps = Session.LapsCount.ToString();
            if (Session.Sectors.Count>0)
            {
                var lastSector = Session.Sectors.Last();
                LastSectorTime = lastSector.Time.ToString("mm\\:ss");
                Distance = $"{Math.Round(Session.Sectors.Count * 0.1f,1)}Km";
                if (Session.BestSector != null) 
                    BestSectorTime = Session.BestSector.Value.Time.ToString("mm\\:ss");
            }
            else
            {
                LastSectorTime = NoValue;
                BestSectorTime = NoValue;
            }
            if (Session.WayPoints.Count>0)
                CurrentSector = $"Currently in {Session.WayPoints.Last().Type.GetSectorName()} sector";
        }

        public ICommand StopCommand => new SimpleCommand(StopLocationService);

        private void StopLocationService()
        {
            _locationService.LocationReceived -= LocationServiceOnLocationReceived;
            _bleLocationService.CheckPointPassed-= BleLocationServiceOnCheckPointPassed;

            _locationService.StopFetchLocation();
            _bleLocationService.StopBleScan();
            
            IsRunning = false;
            InfoLabel = string.Empty;
            
#pragma warning disable 4014
            SaveSessionAndSyncData(true);
#pragma warning restore 4014
        }

        public bool IsRunning
        {
            get => _isRunning;
            private set => SetProperty(ref _isRunning, value);
        }

        public Coordinate LastCoordinate { get; private set; }

        public string InfoLabel
        {
            get => _infoLabel;
            private set => SetProperty(ref _infoLabel, value);
        }

        public string CurrentSector
        {
            get => _currentSector;
            set => SetProperty(ref _currentSector, value);
        }

        public ISession? Session
        {
            get => _currentSession;
            private set
            {
                SetProperty(ref _currentSession, value);
                NotifyPropertyChanged(nameof(CanStart));
            }
        }

        public bool CanStart => Session != null;

        public string LastLapTime
        {
            get => _lastLapTime;
            private set => SetProperty(ref _lastLapTime, value);
        }

        public string Laps    
        {
            get => _laps;
            private set => SetProperty(ref _laps, value);
        }

        public string LastSectorTime
        {
            get => _lastSectorTime;
            private set => SetProperty(ref _lastSectorTime, value);
        }

        public string Distance
        {
            get => _distance;
            private set => SetProperty(ref _distance, value);
        }

        public string BestLapTime
        {
            get => _bestLapTime;
            private set => SetProperty(ref _bestLapTime, value);
        }

        public string BestSectorTime
        {
            get => _bestSectorTime;
            private set => SetProperty(ref _bestSectorTime, value);
        }

        public override void DetachHandlers()
        {
            base.DetachHandlers();
            StopLocationService();
        }

        public override void AttachHandlers()
        {
            base.AttachHandlers();
            CreateSession();
            StartCommand.Execute(null);
        }

        private void CreateSession()
        {
            Session = _trackService.SelectedRink != null
                ? _sessionService.CreateSessionForRink(_trackService.SelectedRink)
                : null;
        }

        private SessionDto? GetSessionDto()
        {
            if (Session == null)
                return null;
            return new SessionDto
            {
                Id = Session.SessionId,
                AccountId = _accountService.UserId,
            };
        }
    }
}