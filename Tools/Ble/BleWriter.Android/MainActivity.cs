using Android.App;
using Android.Content.PM;
using Android.OS;
using BleWriter.Android.Services;
using Sanet.SmartSkating.Droid.Services.Location;
using Sanet.SmartSkating.Dto.Services;
using Sanet.SmartSkating.Services.Storage;

namespace BleWriter.Android
{
    [Activity(Label = "BleWriter", Theme = "@style/MainTheme", MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App(
                new AndroidBleService(
                    new JsonStorageService(),
                    new LocalBleDevicesProvider())
                , new AndroidBleWriter() ));
        }
    }
}