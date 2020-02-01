using Sanet.SmartSkating.ViewModels.Base;

namespace Sanet.SmartSkating.ViewModels
{
    public class LoginViewModel:BaseViewModel
    {
        private string _username = string.Empty;
        private string _password = string.Empty;

        public string Username    
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }
    }
}