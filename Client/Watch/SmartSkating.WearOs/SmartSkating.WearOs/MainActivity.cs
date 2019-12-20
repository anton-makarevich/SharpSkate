using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.Wearable.Activity;
using Android.Views;
using Sanet.SmartSkating.Droid.Utils;
using Sanet.SmartSkating.Services;
using Sanet.SmartSkating.ViewModels;
using Sanet.SmartSkating.WearOs.Services;
using SimpleInjector;

namespace Sanet.SmartSkating.WearOs
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : WearableActivity
    {
        private readonly Container _container = new Container();
        private INavigationService _navigationService;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            
            _container.RegisterModules(this);
            _navigationService = new AndroidNavigationService(this,_container);
            
            Window.SetFlags(WindowManagerFlags.KeepScreenOn, WindowManagerFlags.KeepScreenOn);
            SetContentView(Resource.Layout.activity_main);
            
             var permissions = this.RequestPermissions();
            
            SetAmbientEnabled();

#pragma warning disable 4014
            NavigateToTracks(permissions[0]);
#pragma warning restore 4014
        }
        
        public override void OnRequestPermissionsResult(
            int requestCode, 
            string[] permissions,
            [GeneratedEnum] 
            Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
#pragma warning disable 4014
            NavigateToTracks(grantResults[0]);
#pragma warning restore 4014
        }

        private async Task NavigateToTracks(Permission permission)
        {
            if (permission == Permission.Granted)
                 await _navigationService.NavigateToViewModelAsync<TracksViewModel>();
        }
    }
}