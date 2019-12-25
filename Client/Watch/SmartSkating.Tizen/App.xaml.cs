using Sanet.SmartSkating.ViewModels;
using Sanet.SmartSkating.Xf.Services;
using Sanet.SmartSkating.Xf.Views;
using SimpleInjector;
using Xamarin.Forms;

namespace Sanet.SmartSkating.Xf
{
    public partial class App
    {
        public App(Container container)
        {
            InitializeComponent();

            MainPage = new NavigationPage( new StartView()
            {
                ViewModel = new XamarinFormsNavigationService(container).GetViewModel<StartViewModel>()
            });
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}