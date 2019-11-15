using Sanet.SmartSkating.Services.Location;
using Sanet.SmartSkating.Services.Storage;
using Sanet.SmartSkating.ViewModels;
using Sanet.SmartSkating.Xf.Views;

namespace Sanet.SmartSkating.Xf
{
    public partial class App
    {
        public App()
        {
            InitializeComponent();

            MainPage = new LiveSessionView()
            {
                ViewModel = new LiveSessionViewModel(
                    new EssentialsLocationService(),
                    new JsonStorageService())
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