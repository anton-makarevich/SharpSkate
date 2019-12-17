using Sanet.SmartSkating.ViewModels;
using Sanet.SmartSkating.Xf.Services;
using Sanet.SmartSkating.Xf.Views;
using SimpleInjector;

namespace Sanet.SmartSkating.Xf
{
    public partial class App
    {
        public App(Container container)
        {
            InitializeComponent();

            MainPage = new TracksView()
            {
                ViewModel = new XamarinFormsNavigationService(container).GetViewModel<TracksViewModel>()
            };
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