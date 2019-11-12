using Android.App;
using Android.OS;
using Android.Support.Wearable.Activity;
using Android.Widget;

namespace Sanet.SmartSkating.WearOs
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : WearableActivity
    {
        TextView _textView;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.activity_main);

            _textView = FindViewById<TextView>(Resource.Id.text);
            SetAmbientEnabled();
        }
    }
}