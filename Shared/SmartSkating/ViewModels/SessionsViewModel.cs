using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AsyncAwaitBestPractices.MVVM;
using Sanet.SmartSkating.Dto;
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
        private SessionViewModel? _selectedSession;

        public SessionsViewModel(IApiService apiClient,
            IAccountService accountService,
            ISessionProvider sessionProvider, 
            ITrackService trackService)
        {
            _apiClient = apiClient;
            _accountService = accountService;
            _sessionProvider = sessionProvider;
            _trackService = trackService;
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
            }
        }

        public bool SessionSelected => SelectedSession != null;
        public IAsyncValueCommand StartCommand => new AsyncValueCommand(StartSession);
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
            await NavigationService.NavigateToViewModelAsync<SessionDetailsViewModel>();
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
            Sessions.Clear();
            if (_trackService.SelectedRink == null)
            {
                await _trackService.LoadTracksAsync();
            }
            (await _apiClient.GetSessionsAsync(
                    _accountService.UserId,
                    _trackService.SelectedRink != null,
                    ApiNames.AzureApiSubscriptionKey))
                .Sessions?.OrderBy(f=>f.StartTime).ToList().ForEach(s=>
                {
                    if (_trackService.SelectedRink == null || _trackService.SelectedRink.Id == s.RinkId)
                    {
                        Sessions.Add(new SessionViewModel(s,_trackService.Tracks));
                    }
                });
            if (Sessions.Count == 0 && _trackService.SelectedRink!=null)
            {
                _sessionProvider.CreateSessionForRink(_trackService.SelectedRink);
                await NavigationService.NavigateToViewModelAsync<LiveSessionViewModel>();
            }
        }
    }
}