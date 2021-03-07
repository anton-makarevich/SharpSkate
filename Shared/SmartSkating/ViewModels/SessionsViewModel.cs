using System.Collections.ObjectModel;
using System.Threading.Tasks;
using AsyncAwaitBestPractices.MVVM;
using Sanet.SmartSkating.Dto;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Services.Account;
using Sanet.SmartSkating.Services.Api;
using Sanet.SmartSkating.Services.Tracking;
using Sanet.SmartSkating.ViewModels.Base;

namespace Sanet.SmartSkating.ViewModels
{
    public class SessionsViewModel:BaseViewModel
    {
        private readonly IApiService _apiClient;
        private readonly IAccountService _accountService;
        private readonly ISessionProvider _sessionProvider;
        private readonly ITrackService _trackService;
        private SessionDto? _selectedSession;

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

        public ObservableCollection<SessionDto> Sessions { get; } =
            new ObservableCollection<SessionDto>();

        public SessionDto? SelectedSession
        {
            get => _selectedSession;
            private set => SetProperty(ref _selectedSession, value);
        }

        public bool SessionSelected => SelectedSession != null;
        public IAsyncValueCommand StartCommand => new AsyncValueCommand(StartSession);
        public bool CanStart => SessionSelected && _trackService.SelectedRink != null;

        private async ValueTask StartSession()
        {
            if (SelectedSession == null || _trackService.SelectedRink == null)
                return;
            _sessionProvider.SetActiveSession(SelectedSession,_trackService.SelectedRink);
            await NavigationService.NavigateToViewModelAsync<LiveSessionViewModel>();
        }

        public override void AttachHandlers()
        {
            base.AttachHandlers();
#pragma warning disable 4014
            GetSessions();
#pragma warning restore 4014
        }

        private async Task GetSessions()
        {
            Sessions.Clear();
            (await _apiClient.GetSessionsAsync(
                    _accountService.UserId,
                    _trackService.SelectedRink!=null,
                    ApiNames.AzureApiSubscriptionKey))
                .Sessions.ForEach(s=>
                {
                    if (_trackService.SelectedRink == null || _trackService.SelectedRink.Id == s.RinkId)
                    {
                        Sessions.Add(s);
                    }
                });
        }

        public void SelectSession(SessionDto sessionToSelect)
        {
            SelectedSession = sessionToSelect;
            NotifyPropertyChanged(nameof(SessionSelected));
            NotifyPropertyChanged(nameof(CanStart));
        }
    }
}