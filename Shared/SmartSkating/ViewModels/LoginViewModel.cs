using System.Windows.Input;
using Sanet.SmartSkating.Dto.Services.Account;
using Sanet.SmartSkating.Models;
using Sanet.SmartSkating.ViewModels.Base;

namespace Sanet.SmartSkating.ViewModels
{
    public class LoginViewModel:BaseViewModel
    {
        private readonly ILoginService _loginService;
        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _validationMessage = string.Empty;

        public LoginViewModel(ILoginService loginService)
        {
            _loginService = loginService;
        }

        public string Username
        {
            get => _username;
            set
            {
                SetProperty(ref _username, value);
                ValidationMessage = string.Empty;
                NotifyPropertyChanged(nameof(CanLogin));
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                SetProperty(ref _password, value);
                ValidationMessage = string.Empty;
                NotifyPropertyChanged(nameof(CanLogin));
            }
        }

        public bool CanLogin => 
            !string.IsNullOrEmpty(Username)
            && !string.IsNullOrEmpty(Password);

        public ICommand LoginCommand => new SimpleCommand( LoginAsync);

        private async void LoginAsync()
        {
            if (CanLogin)
            {
                var account = await _loginService.LoginUserAsync(Username, Password);
                if (account != null)
                {
                    if (NavigationService != null)
                        await NavigationService.NavigateToViewModelAsync<SessionsViewModel>();
                }
                else
                {
                    ValidationMessage = CheckCredentialsMessage;
                }
            }
            else
                _validationMessage = CheckCredentialsMessage;
        }

        public string ValidationMessage    
        {
            get => _validationMessage;
            private set => SetProperty(ref _validationMessage, value);
        }

        public const string CheckCredentialsMessage = "Please check your credentials";

        public override void AttachHandlers()
        {
            base.AttachHandlers();
            NotifyPropertyChanged(nameof(CanLogin));
        }
    }
}