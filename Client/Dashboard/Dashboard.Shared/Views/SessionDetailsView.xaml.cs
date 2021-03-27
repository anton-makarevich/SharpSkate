using Microsoft.Extensions.DependencyInjection;
using Sanet.SmartSkating.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Sanet.SmartSkating.Dashboard.Views
{
    public sealed partial class SessionDetailsView : Page
    {
        private SessionDetailsViewModel? _viewModel;

        public SessionDetailsView()
        {
            this.InitializeComponent();
            var container = ((App)Application.Current).Container;
            var vm = ActivatorUtilities
                .GetServiceOrCreateInstance(container, typeof(SessionDetailsViewModel)) as SessionDetailsViewModel;
            vm?.SetNavigationService(((App)Application.Current).NavigationService);
            vm?.AttachHandlers();
            ViewModel = vm;
        }

        public SessionDetailsViewModel? ViewModel
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
