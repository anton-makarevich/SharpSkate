using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AsyncAwaitBestPractices.MVVM;
using Sanet.SmartSkating.Dto.Services;
using Sanet.SmartSkating.Services.Account;
using Sanet.SmartSkating.Services.Api;
using Sanet.SmartSkating.Services.Tracking;
using Sanet.SmartSkating.ViewModels.Base;
using Sanet.SmartSkating.ViewModels.Wrappers;

namespace Sanet.SmartSkating.ViewModels
{
    public class SessionsViewModel:BaseViewModel
    {
        private readonly IApiService _apiClient;
        private readonly IAccountService _accountService;
        private readonly ISessionProvider _sessionProvider;
        private readonly ITrackService _trackService;
        private readonly IDataSyncService _dataSyncService;
        private readonly IConfigService _configService;
        private SessionViewModel? _selectedSession;

        private bool? _onlyActiveSessions;

        public SessionsViewModel(IApiService apiClient,
            IAccountService accountService,
            ISessionProvider sessionProvider,
            ITrackService trackService,
            IDataSyncService dataSyncService,
            IConfigService configService)
        {
            _apiClient = apiClient;
            _accountService = accountService;
            _sessionProvider = sessionProvider;
            _trackService = trackService;
            _dataSyncService = dataSyncService;
            _configService = configService;
        }

        public ObservableCollection<SessionViewModel> Sessions { get; } =
            new ObservableCollection<SessionViewModel>();

        public SessionViewModel? SelectedSession
        {
            get => _selectedSession;
            set
            {
                SetProperty(ref _selectedSession, value);
                NotifyPropertyChanged(nameof(SessionSelected));
                NotifyPropertyChanged(nameof(CanStart));
                NotifyPropertyChanged(nameof(CanOpenSessionDetails));
            }
        }

        public bool OnlyActiveSessions  {
            get
            {
                if (_onlyActiveSessions.HasValue)
                {
                    return _onlyActiveSessions.Value;
                }
                return _trackService.SelectedRink != null;
            }
            set => SetProperty(ref _onlyActiveSessions, value);
        }

        public bool SessionSelected => SelectedSession != null;
        public IAsyncValueCommand StartCommand => new AsyncValueCommand(StartSession);
        public IAsyncValueCommand StartNewCommand => new AsyncValueCommand(StartNewSession);
        public IAsyncValueCommand OpenDetailsCommand => new AsyncValueCommand(OpenSessionDetails);

        public bool CanStart => SessionSelected && _trackService.SelectedRink != null;
        public bool CanOpenSessionDetails => SessionSelected && SelectedSession?.RinkName != "Unknown";

        private async ValueTask OpenSessionDetails()
        {
            if (SelectedSession == null || string.IsNullOrEmpty(SelectedSession.RinkName))
                return;
            _trackService.SelectRinkByName(SelectedSession.RinkName);

            if (_trackService.SelectedRink == null)
            {
                return;
            }

            _sessionProvider.SetActiveSession(SelectedSession.Session, _trackService.SelectedRink);
            var newSessionsDetailsViewModel = NavigationService.GetNewViewModel<SessionDetailsViewModel>();
            await NavigationService.NavigateToViewModelAsync(newSessionsDetailsViewModel);
        }

        private async ValueTask StartSession()
        {
            if (SelectedSession == null || _trackService.SelectedRink == null)
                return;
            _sessionProvider.SetActiveSession(SelectedSession.Session,_trackService.SelectedRink);
            await NavigationService.NavigateToViewModelAsync<LiveSessionViewModel>();
        }

        public override void AttachHandlers()
        {
            base.AttachHandlers();
#pragma warning disable 4014
            GetSessions();
#pragma warning restore 4014
        }

        private async ValueTask GetSessions()
        {
            if (string.IsNullOrEmpty(_accountService.UserId))
                return;
            foreach (var session in Sessions)
            {
                session.SessionUpdated -= SessionOnSessionUpdated;
            }
            Sessions.Clear();
            if (_trackService.SelectedRink == null)
            {
                await _trackService.LoadTracksAsync();
            }
            (await _apiClient.GetSessionsAsync(
                    _accountService.UserId,
                    OnlyActiveSessions,
                    _configService.AzureApiSubscriptionKey))
                .Sessions?.OrderBy(f=>f.StartTime).ToList().ForEach(s=>
                {
                    if (_trackService.SelectedRink != null && _trackService.SelectedRink.Id != s.RinkId) return;
                    var session = new SessionViewModel(s, _trackService.Tracks, _dataSyncService);
                    session.SessionUpdated+= SessionOnSessionUpdated;
                    Sessions.Add(session);
                });
            if (Sessions.Count == 0 && _trackService.SelectedRink!=null)
            {
                await StartNewSession();
            }
        }

        private void SessionOnSessionUpdated()
        {
#pragma warning disable CS4014
            GetSessions();
#pragma warning restore CS4014
        }

        private async ValueTask StartNewSession()
        {
            if (Sessions.Count == 0 && _trackService.SelectedRink != null)
            {
                _sessionProvider.CreateSessionForRink(_trackService.SelectedRink);
                await NavigationService.NavigateToViewModelAsync<LiveSessionViewModel>();
            }
        }
    }
}
