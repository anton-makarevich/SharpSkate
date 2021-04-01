using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Acr.UserDialogs;
using Sanet.SmartSkating.Dto.Services;
using Sanet.SmartSkating.Models.Training;
using Sanet.SmartSkating.Services.Tracking;

namespace Sanet.SmartSkating.ViewModels
{
    public class SessionDetailsViewModel:LiveSessionViewModel
    {
        private string _finalSessionTime = NoValue;
        private ObservableCollection<Lap> _lapsData = new ObservableCollection<Lap>();

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
            if (SessionManager.CurrentSession != null && SessionManager.CurrentSession.LapsCount != LapsData.Count)
            {
                foreach(var lap in SessionManager.CurrentSession.Laps)
                {
                    if (!LapsData.Contains(lap))
                        LapsData.Add(lap);
                }
            }
        }
        
        public ObservableCollection<Lap> LapsData 
        {
            get => _lapsData;
            private set => SetProperty(ref _lapsData, value);
        }

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
