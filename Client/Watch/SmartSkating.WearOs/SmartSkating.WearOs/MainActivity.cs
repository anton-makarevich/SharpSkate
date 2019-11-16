using System;
using System.ComponentModel;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.Wearable.Activity;
using Android.Views;
using Android.Widget;
using Sanet.SmartSkating.Services.Location;
using Sanet.SmartSkating.Services.Storage;
using Sanet.SmartSkating.ViewModels;

namespace Sanet.SmartSkating.WearOs
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : WearableActivity
    {
        private LiveSessionViewModel _viewModel;
        
        private TextView _textView;
        private Button _startButton;
        private Button _stopButton;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Window.SetFlags(WindowManagerFlags.KeepScreenOn, WindowManagerFlags.KeepScreenOn);
            Xamarin.Essentials.Platform.Init(this, bundle);
            SetContentView(Resource.Layout.activity_main);

            _textView = FindViewById<TextView>(Resource.Id.text);
            _startButton = FindViewById<Button>(Resource.Id.startButton);
            _stopButton = FindViewById<Button>(Resource.Id.stopButton);

            SetViewModel();
            
            _startButton.Click+= StartButtonOnClick;
            _stopButton.Click+= StopButtonOnClick;
            
            SetAmbientEnabled();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _startButton.Click-= StartButtonOnClick;
            _stopButton.Click-= StopButtonOnClick;
            _viewModel.PropertyChanged-= ViewModelOnPropertyChanged;
        }

        private void StopButtonOnClick(object sender, EventArgs e)
        {
            _viewModel.StopCommand.Execute(null);
        }

        private void StartButtonOnClick(object sender, EventArgs e)
        {
            _viewModel.StartCommand.Execute(true);
        }

        private void SetViewModel()
        {
            _viewModel = new LiveSessionViewModel(new EssentialsLocationService(), new JsonStorageService());
            _viewModel.PropertyChanged+= ViewModelOnPropertyChanged;
            
            UpdateTextState();
            UpdateButtonsState();
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_viewModel.InfoLabel))
            {
                UpdateTextState();
                return;
            }

            if (e.PropertyName == nameof(_viewModel.IsRunning))
            {
                UpdateButtonsState();
            }
        }

        private void UpdateButtonsState()
        {
            _startButton.Visibility = (_viewModel.IsRunning) ? ViewStates.Gone : ViewStates.Visible;
            _stopButton.Visibility = (_viewModel.IsRunning) ? ViewStates.Visible : ViewStates.Gone;
        }

        private void UpdateTextState()
        {
            _textView.Text = _viewModel.InfoLabel;
        }
        
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}