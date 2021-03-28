using System;
using System.Linq;
using Acr.UserDialogs;
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
            if (ForceUiUpdate)
            {
                UpdateFinalTime();
            }
            base.UpdateUi();
        }

        public override bool ForceUiUpdate => !SessionManager.IsRunning 
                                              && SessionManager.IsRemote  
                                              && FinalSessionTime == NoValue;

        private void UpdateFinalTime()
        {
            if (SessionManager.CurrentSession == null) return;
            var finalTime = SessionManager.CurrentSession.WayPoints.Last().Date;
            var time = finalTime.Subtract(SessionManager.CurrentSession.StartTime);
            FinalSessionTime = time.ToString(TotalTimeFormat);
        }

        public override void AttachHandlers()
        {
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
        }
    }
}
