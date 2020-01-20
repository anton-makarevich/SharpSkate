using System;
using System.ComponentModel;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.Wearable.Activity;
using Android.Views;
using Android.Widget;
using Sanet.SmartSkating.Droid.Utils;
using Sanet.SmartSkating.Services;
using Sanet.SmartSkating.ViewModels;
using Sanet.SmartSkating.WearOs.Services;
using Container = SimpleInjector.Container;

namespace Sanet.SmartSkating.WearOs
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : WearableActivity
    {
        private readonly Container _container = new Container();
        private INavigationService? _navigationService;
        private StartViewModel? _viewModel;
        
        private TextView? _rinkNameText;
        private TextView? _infoText;

        private Button? _startButton;
        private Button? _selectRinkButton;
        
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            
            _container.RegisterModules(this);
            _navigationService = new AndroidNavigationService(this,_container);
            
            Window.SetFlags(WindowManagerFlags.KeepScreenOn, WindowManagerFlags.KeepScreenOn);
            SetContentView(Resource.Layout.activity_main);
            
            _rinkNameText = FindViewById<TextView>(Resource.Id.track_name);
            _infoText = FindViewById<TextView>(Resource.Id.info_label);
            
            _startButton = FindViewById<Button>(Resource.Id.startButton);
            _selectRinkButton = FindViewById<Button>(Resource.Id.select_rink_button);
            
            _startButton.Click+= StartButtonOnClick;
            _selectRinkButton.Click+= SelectRinkButtonOnClick;
            
            var permissions = this.RequestPermissions();
            
            SetAmbientEnabled();

            _viewModel = _container.GetInstance<StartViewModel>();
            _viewModel.PropertyChanged += ViewModelOnPropertyChanged;
            
            InitGeoServices(permissions[0]);
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_viewModel == null) return;
            if (e.PropertyName == nameof(_viewModel.CanStart))
            {
                if (_startButton != null) _startButton.Enabled = _viewModel.CanStart;
            }
            else if (e.PropertyName == nameof(_viewModel.InfoLabel))
            {
                if (_infoText != null) _infoText.Text = _viewModel.InfoLabel;
            }
            else if (e.PropertyName == nameof(_viewModel.IsInitializingGeoServices))
            {
                if (_selectRinkButton != null) _selectRinkButton.Enabled = !_viewModel.IsInitializingGeoServices;
            }
            else if (e.PropertyName == nameof(_viewModel.RinkName))
            {
                if (_rinkNameText != null) _rinkNameText.Text = _viewModel.RinkName;
            }
            else if (e.PropertyName == nameof(_viewModel.IsRinkSelected))
            {
                if (_viewModel.IsRinkSelected)
                {
                    if (_rinkNameText != null) _rinkNameText.Visibility = ViewStates.Visible;
                    if (_selectRinkButton != null) _selectRinkButton.Visibility = ViewStates.Gone;
                }
                else
                {
                    if (_rinkNameText != null) _rinkNameText.Visibility = ViewStates.Gone;
                    if (_selectRinkButton != null) _selectRinkButton.Visibility = ViewStates.Visible;
                }
            }
        }

        private async void SelectRinkButtonOnClick(object sender, EventArgs e)
        {
            if (_navigationService != null) await _navigationService.NavigateToViewModelAsync<TracksViewModel>();
        }

        private async void StartButtonOnClick(object sender, EventArgs e)
        {
            if (_navigationService != null) await _navigationService.NavigateToViewModelAsync<LiveSessionViewModel>();
        }

        public override void OnRequestPermissionsResult(
            int requestCode, 
            string[] permissions,
            [GeneratedEnum] 
            Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            InitGeoServices(grantResults[0]);
        }

        private void InitGeoServices(Permission permission)
        {
            if (permission == Permission.Granted)
                _viewModel?.AttachHandlers();
        }
    }
}