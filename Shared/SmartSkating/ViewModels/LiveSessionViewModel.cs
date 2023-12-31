using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using Sanet.SmartSkating.Dto.Services;
using Sanet.SmartSkating.Models;
using Sanet.SmartSkating.Models.EventArgs;
using Sanet.SmartSkating.Models.Training;
using Sanet.SmartSkating.Services.Narration;
using Sanet.SmartSkating.Services.Tracking;
using Sanet.SmartSkating.ViewModels.Base;

namespace Sanet.SmartSkating.ViewModels
{
    public class LiveSessionViewModel:BaseViewModel
    {
        public const string NoValue = "- - -";
        public const string TotalTimeFormat = "h\\:mm\\:ss";
        public const string LapTimeFormat = "mm\\:ss";

        protected readonly ISessionManager SessionManager;
        private readonly IDateProvider _dateProvider;
        private readonly INarratorService? _narratorService;
        private readonly IUserDialogs _userDialogs;
        
        private string _infoLabel = "";
        private string _currentSector = NoValue;
        private string _lastLapTime = NoValue;
        private string _laps = "0";
        private string _lastSectorTime = NoValue;
        private string _distance = "";
        private string _totalTime = "";
        private string _bestLapTime = NoValue;
        private string _bestSectorTime = NoValue;

        public LiveSessionViewModel(ISessionManager sessionManager,
            IDateProvider dateProvider,
            IUserDialogs userDialogs,
            INarratorService? narratorService=null)
        {
            SessionManager = sessionManager;
            _dateProvider = dateProvider;
            _narratorService = narratorService;
            _userDialogs = userDialogs;

            TotalTime = new TimeSpan().ToString(TotalTimeFormat);
        }

        public ICommand StartCommand => new SimpleCommand(async() =>
        {
            await SessionManager.StartSession();
            if (_narratorService != null && SessionManager.CurrentSession != null)
            {
                SessionManager.CurrentSession.LapPassed += SpeakLapTime;
            }
#pragma warning disable 4014
            TrackTime();
#pragma warning restore 4014
            UpdateButtonsState();
        });

        private void SpeakLapTime(object sender, LapEventArgs e)
        {
            if (_narratorService == null) return;
            var textToSpeak = $"Lap number {e.Lap.Number} - {e.Lap.Time.ToString(LapTimeFormat)}";
            if (e.IsBest.HasValue && e.IsBest.Value)
                textToSpeak += " - Best lap!";
            _narratorService.SpeakText(textToSpeak);
        }

        public ICommand StopCommand => new SimpleCommand(async () =>
        {
            var isConfirmed = await _userDialogs.ConfirmAsync("Do you want to stop session");
            if (!isConfirmed) return;
            if (SessionManager.CurrentSession != null)
            {
                SessionManager.CurrentSession.LapPassed -= SpeakLapTime;
            }
            SessionManager.StopSession();
            InfoLabel = "";
            UpdateButtonsState();
            NotifyPropertyChanged(nameof(CanStart));
        });

        public async Task TrackTime()
        {
            do
            {
                if (SessionManager.CurrentSession is null) continue;
                UpdateUi();
                await Task.Delay(1000);
            } while (SessionManager.IsRunning && IsActive);
        }

        public string TotalTime
        {
            get => _totalTime;
            private set => SetProperty(ref _totalTime, value);
        }

        public virtual void UpdateUi()
        {
            if (!IsRunning && !ForceUiUpdate)
                return;
            if (SessionManager.CurrentSession == null) return;
            
            var time = _dateProvider.Now().Subtract(SessionManager.CurrentSession.StartTime);
            TotalTime = time.ToString(TotalTimeFormat);
            
            InfoLabel = SessionManager.CurrentSession.LastCoordinate.ToString();

            if (SessionManager.CurrentSession.LapsCount > 0)
            {
                LastLapTime = SessionManager.CurrentSession.LastLapTime.ToString(LapTimeFormat);
                BestLapTime = SessionManager.CurrentSession.BestLapTime.ToString(LapTimeFormat);
            }
            else
            {
                LastLapTime = NoValue;
                BestLapTime = NoValue;
            }

            Laps = SessionManager.CurrentSession.LapsCount.ToString();
            if (SessionManager.CurrentSession.Sectors.Count > 0)
            {
                var lastSector = SessionManager.CurrentSession.Sectors.Last();
                LastSectorTime = lastSector.Time.ToString(LapTimeFormat);
                Distance = $"{Math.Round(SessionManager.CurrentSession.Sectors.Count * 0.1f, 1)}Km";
                if (SessionManager.CurrentSession.BestSector != null)
                    BestSectorTime = SessionManager.CurrentSession.BestSector.Value.Time.ToString(LapTimeFormat);
            }
            else
            {
                LastSectorTime = NoValue;
                BestSectorTime = NoValue;
            }

            if (SessionManager.CurrentSession.WayPoints.Count > 0)
                CurrentSector =
                    $"Currently in {SessionManager.CurrentSession.WayPoints.Last().Type.GetSectorName()} sector";
        }

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

        public bool CanStart => SessionManager.CanStart;

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

        public bool IsRunning => SessionManager.IsRunning;
        
        private void UpdateButtonsState()
        {
            NotifyPropertyChanged(nameof(IsStartVisible));
            NotifyPropertyChanged(nameof(IsStopVisible));
        }
        
        public bool IsStartVisible => SessionManager.CanStart && !SessionManager.IsRemote;
        public bool IsStopVisible => IsRunning && !SessionManager.IsRemote;
        public virtual bool ForceUiUpdate { get; } = false;

        public override void AttachHandlers()
        {
            base.AttachHandlers();
            IsActive = true;
            SessionManager.CheckSession();
            if (!SessionManager.IsRunning) return;
            
#pragma warning disable 4014
            TrackTime();
#pragma warning restore 4014
        }

        public override void DetachHandlers()
        {
            base.DetachHandlers();
            if (SessionManager.CurrentSession != null)
            {
                SessionManager.CurrentSession.LapPassed -= SpeakLapTime;
            }
            IsActive = false;
        }
    }
}
