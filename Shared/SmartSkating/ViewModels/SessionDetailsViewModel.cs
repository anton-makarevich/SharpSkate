using System;
using System.Collections.Generic;
using System.Linq;
using Acr.UserDialogs;
using Microcharts;
using Sanet.SmartSkating.Dto.Services;
using Sanet.SmartSkating.Services.Tracking;

namespace Sanet.SmartSkating.ViewModels
{
    public class SessionDetailsViewModel:LiveSessionViewModel
    {
        private string _finalSessionTime = NoValue;

        public SessionDetailsViewModel(
            ISessionManager sessionManager,
            IDateProvider dateProvider,
            IUserDialogs userDialogs) : base(sessionManager, dateProvider, userDialogs)
        {
        }

        public string FinalSessionTime
        {
            get => _finalSessionTime;
            private set => SetProperty(ref _finalSessionTime, value);
        }

        public override void UpdateUi()
        {
            base.UpdateUi();
            if (!ForceUiUpdate && !SessionManager.IsRunning) return;
            UpdateFinalTime();
            UpdateChart();
        }

        private void UpdateChart()
        {
            if (SessionManager.CurrentSession == null ||
                SessionManager.CurrentSession.LapsCount == LapsData.Count) return;
            foreach(var lap in SessionManager.CurrentSession.Laps)
            {
                if (lap.Number>LapsData.Count)
                     LapsData.Add(new ChartEntry((float)lap.Time.TotalMilliseconds)
                     {
                         Label = lap.Number.ToString(),
                         ValueLabel = lap.Time.ToString(LapTimeFormat),
                     });
            }
            NotifyPropertyChanged(nameof(Chart));
        }

        public Chart Chart => new BarChart() { Entries = LapsData };
        public List<ChartEntry> LapsData { get; } = new List<ChartEntry>();

        public override bool ForceUiUpdate => !SessionManager.IsRunning
                                              && SessionManager.IsRemote
                                              && FinalSessionTime == NoValue;

        private void UpdateFinalTime()
        {
            if (SessionManager.CurrentSession?.WayPoints == null
                || SessionManager.CurrentSession?.WayPoints.Count == 0) return;
#pragma warning disable 8602
            var finalTime = SessionManager.CurrentSession.WayPoints.Last().Date;
#pragma warning restore 8602
            var time = finalTime.Subtract(SessionManager.CurrentSession.StartTime);
            FinalSessionTime = time.ToString(TotalTimeFormat);
        }

        public override void AttachHandlers()
        {
            UpdateChart();
            base.AttachHandlers();
            SessionManager.SessionUpdated += OnSessionUpdate;
        }

        public override void DetachHandlers()
        {
            base.DetachHandlers();
            SessionManager.SessionUpdated -= OnSessionUpdate;
        }

        public void OnSessionUpdate(object? sender, EventArgs e)
        {
#pragma warning disable 4014
            TrackTime();
#pragma warning restore 4014
        }
    }
}
