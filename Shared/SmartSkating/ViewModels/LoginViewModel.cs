using System.Windows.Input;
using Sanet.SmartSkating.Models;
using Sanet.SmartSkating.Services.Account;
using Sanet.SmartSkating.ViewModels.Base;

namespace Sanet.SmartSkating.ViewModels
{
    public class LoginViewModel:BaseViewModel
    {
        private readonly ILoginService _loginService;
        private string _username = string.Empty;
        private string _password = string.Empty;

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
                NotifyPropertyChanged(nameof(CanLogin));
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                SetProperty(ref _password, value);
                NotifyPropertyChanged(nameof(CanLogin));
            }
        }

        public bool CanLogin => 
            !string.IsNullOrEmpty(Username)
            && !string.IsNullOrEmpty(Password);

        public ICommand LoginCommand => new SimpleCommand(async () =>
        {
            await _loginService.LoginUserAsync(Username,Password);
        });
    }
}