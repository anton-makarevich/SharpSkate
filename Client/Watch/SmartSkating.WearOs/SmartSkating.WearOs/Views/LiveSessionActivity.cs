using System;
using System.ComponentModel;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Sanet.SmartSkating.Droid.Services.Location;
using Sanet.SmartSkating.Dto.Services;
using Sanet.SmartSkating.Services.Storage;
using Sanet.SmartSkating.Services.Tracking;
using Sanet.SmartSkating.ViewModels;

namespace Sanet.SmartSkating.WearOs.Views
{
    [Activity]
    public class LiveSessionActivity:BaseActivity<LiveSessionViewModel>
    {
        private TextView? _textView;
        private Button? _startButton;
        private Button? _stopButton;
        
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.activity_session);
            
            _textView = FindViewById<TextView>(Resource.Id.text);
            _startButton = FindViewById<Button>(Resource.Id.startButton);
            _stopButton = FindViewById<Button>(Resource.Id.stopButton);
            
            _startButton.Click+= StartButtonOnClick;
            _stopButton.Click+= StopButtonOnClick;

            SetViewModel();
            
            SetAmbientEnabled();
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (_startButton != null) _startButton.Click -= StartButtonOnClick;
            if (_stopButton != null) _stopButton.Click -= StopButtonOnClick;
            if (ViewModel != null) ViewModel.PropertyChanged -= ViewModelOnPropertyChanged;
        }
        
        private void StopButtonOnClick(object sender, EventArgs e)
        {
            ViewModel?.StopCommand.Execute(null);
        }

        private void StartButtonOnClick(object sender, EventArgs e)
        {
            ViewModel?.StartCommand.Execute(true);
        }
        
        
        private void SetViewModel()
        {
            var storageService = new JsonStorageService();
            ViewModel = new LiveSessionViewModel(
                new LocationManagerService(this), 
                storageService,
                new TrackService(new LocalTrackProvider()),
                new SessionService());
            ViewModel.PropertyChanged+= ViewModelOnPropertyChanged;
            
#if DEBUG
            storageService.LoadAllCoordinatesAsync();
#endif
            
            UpdateTextState();
            UpdateButtonsState();
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModel.InfoLabel))
            {
                UpdateTextState();
                return;
            }

            if (e.PropertyName == nameof(ViewModel.IsRunning))
            {
                UpdateButtonsState();
            }
        }

        private void UpdateButtonsState()
        {
            if (_startButton != null)
                _startButton.Visibility = 
                    (ViewModel != null && ViewModel.IsRunning)
                        ? ViewStates.Gone 
                        : ViewStates.Visible;
            if (_stopButton != null)
                _stopButton.Visibility = (ViewModel != null && ViewModel.IsRunning) 
                    ? ViewStates.Visible 
                    : ViewStates.Gone;
        }

        private void UpdateTextState()
        {
            if (_textView != null) _textView.Text = ViewModel?.InfoLabel;
        }
    }
}