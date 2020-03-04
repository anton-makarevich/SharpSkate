using Sanet.SmartSkating.Dashboard.Views;
using Sanet.SmartSkating.ViewModels;
using Sanet.SmartSkating.Xf.Services;
using SimpleInjector;
using Xamarin.Forms;

namespace Sanet.SmartSkating.Dashboard
{
    public partial class App
    {
        public App(Container container)
        {
            InitializeComponent();

            MainPage = new NavigationPage( new LoginView()
            {
                ViewModel = new XamarinFormsNavigationService(container).GetViewModel<LoginViewModel>()
            });
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
