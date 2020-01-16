using BleWriter.Services;
using BleWriter.ViewModels;
using BleWriter.Views;
using Sanet.SmartSkating.Services.Location;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace BleWriter
{
    public partial class App : Application
    {
        public App(IBleLocationService bleLocationService, IBleWriterService bleWriterService)
        {
            InitializeComponent();
            
            MainPage = new BleWriterView
            {
                BindingContext = new BleWriterViewModel(bleLocationService,bleWriterService)
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