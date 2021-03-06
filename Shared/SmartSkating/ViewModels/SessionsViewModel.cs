using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Sanet.SmartSkating.Dto;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Services.Account;
using Sanet.SmartSkating.Services.Api;
using Sanet.SmartSkating.ViewModels.Base;

namespace Sanet.SmartSkating.ViewModels
{
    public class SessionsViewModel:BaseViewModel
    {
        private readonly IApiService _apiClient;
        private readonly IAccountService _accountService;

        public SessionsViewModel(IApiService apiClient, IAccountService accountService)
        {
            _apiClient = apiClient;
            _accountService = accountService;
        }

        public ObservableCollection<SessionDto> Sessions { get; } =
            new ObservableCollection<SessionDto>();

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
            (await _apiClient.GetSessionsAsync(_accountService.UserId, ApiNames.AzureApiSubscriptionKey))
                .Sessions.ForEach(s=>Sessions.Add(s));
        }
    }
}