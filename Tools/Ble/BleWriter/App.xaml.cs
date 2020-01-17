using BleWriter.Services;
using BleWriter.ViewModels;
using BleWriter.Views;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace BleWriter
{
    public partial class App
    {
        public App(IBleWriterService bleWriterService)
        {
            InitializeComponent();
            
            MainPage = new BleWriterView
            {
                BindingContext = new BleWriterViewModel(bleWriterService)
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