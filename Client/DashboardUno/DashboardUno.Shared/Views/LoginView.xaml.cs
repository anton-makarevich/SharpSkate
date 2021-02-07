using Microsoft.Extensions.DependencyInjection;
using Sanet.SmartSkating.ViewModels;
using Windows.UI.Xaml.Controls;

namespace DashboardUno.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginView : Page
    {
        private LoginViewModel _viewModel;

        public LoginView()
        {
            this.InitializeComponent();
            var container = ((App)App.Current).Container;
            var vm = ActivatorUtilities.GetServiceOrCreateInstance(container, typeof(LoginViewModel)) as LoginViewModel;
            vm.SetNavigationService(((App)App.Current).NavigationService);
            ViewModel = vm;
        }

        public LoginViewModel ViewModel
        {
            get => _viewModel;
            private set
            {
                _viewModel = value;
                DataContext = _viewModel;
            }
        }
    }
}
