using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Sanet.SmartSkating.Models;
using Sanet.SmartSkating.Models.Training;
using Sanet.SmartSkating.Services.Tracking;
using Sanet.SmartSkating.ViewModels.Base;

namespace Sanet.SmartSkating.ViewModels
{
    public class LiveSessionViewModel:BaseViewModel
    {
        public const string NoValue = "- - -";
        public const string TotalTimeFormat = "h\\:mm\\:ss";

        private readonly ISessionService _sessionService;
        private string _infoLabel = string.Empty;
        private string _currentSector = NoValue;
        private string _lastLapTime = string.Empty;
        private string _laps = string.Empty;
        private string _lastSectorTime = string.Empty;
        private string _distance = string.Empty;
        private string _totalTime = string.Empty;
        private string _bestLapTime = string.Empty;
        private string _bestSectorTime = string.Empty;

        public LiveSessionViewModel( ISessionService sessionService)
        {
            _sessionService = sessionService;

            TotalTime = new TimeSpan().ToString(TotalTimeFormat);
        }

        public ICommand StartCommand => new SimpleCommand(async() =>
        {
            await _sessionService.StartSession();
            _sessionService.CurrentSession?.SetStartTime(DateTime.UtcNow);
        });


        private async Task TrackTime()
        {
            do
            {
                await Task.Delay(1000);
                if (_sessionService.CurrentSession == null) continue;
                var time = DateTime.UtcNow.Subtract(_sessionService.CurrentSession.StartTime);
                TotalTime = time.ToString(TotalTimeFormat);
                UpdateUi();
            } while (IsActive);
        }

        public string TotalTime
        {
            get => _totalTime;
            private set => SetProperty(ref _totalTime, value);
        }

        public void UpdateUi()
        {
            if (_sessionService.CurrentSession == null) return;
            InfoLabel = _sessionService.CurrentSession.LastCoordinate.ToString();

            if (_sessionService.CurrentSession.LapsCount > 0)
            {
                LastLapTime = _sessionService.CurrentSession.LastLapTime.ToString(TotalTimeFormat);
                BestLapTime = _sessionService.CurrentSession.BestLapTime.ToString(TotalTimeFormat);
            }
            else
            {
                LastLapTime = NoValue;
                BestLapTime = NoValue;
            }

            Laps = _sessionService.CurrentSession.LapsCount.ToString();
            if (_sessionService.CurrentSession.Sectors.Count>0)
            {
                var lastSector = _sessionService.CurrentSession.Sectors.Last();
                LastSectorTime = lastSector.Time.ToString("mm\\:ss");
                Distance = $"{Math.Round(_sessionService.CurrentSession.Sectors.Count * 0.1f,1)}Km";
                if (_sessionService.CurrentSession.BestSector != null)
                    BestSectorTime = _sessionService.CurrentSession.BestSector.Value.Time.ToString("mm\\:ss");
            }
            else
            {
                LastSectorTime = NoValue;
                BestSectorTime = NoValue;
            }
            if (_sessionService.CurrentSession.WayPoints.Count>0)
                CurrentSector = $"Currently in {_sessionService.CurrentSession.WayPoints.Last().Type.GetSectorName()} sector";
        }

        public ICommand StopCommand => new SimpleCommand(() =>
        {
            _sessionService.StopSession();
            InfoLabel = "";
        });

        public bool IsActive { get; private set; }

        public string InfoLabel
        {
            get => _infoLabel;
            private set => SetProperty(ref _infoLabel, value);
        }

        public string CurrentSector
        {
            get => _currentSector;
            private set => SetProperty(ref _currentSector, value);
        }

        public bool CanStart => _sessionService.CurrentSession != null;

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

        public override void AttachHandlers()
        {
            base.AttachHandlers();
            IsActive = true;
#pragma warning disable 4014
            TrackTime();
#pragma warning restore 4014
        }

        public override void DetachHandlers()
        {
            base.DetachHandlers();
            IsActive = false;
        }
    }
}
