using System;
using System.ComponentModel;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Sanet.SmartSkating.ViewModels;
using Sanet.SmartSkating.WearOs.Services;
using Sanet.SmartSkating.WearOs.Views.Components;

namespace Sanet.SmartSkating.WearOs.Views
{
    [Activity]
    public class LiveSessionActivity:BaseActivity<LiveSessionViewModel>
    {
        private TextView? _elapsedTemText;
        private TextView? _distanceText;
        private TitledLabelView? _lapsText;
        private TitledLabelView? _lastLapText;
        private TitledLabelView? _bestLapText;
        private TitledLabelView? _lastSectorText;
        private TitledLabelView? _bestSectorText;
        
        private Button? _startButton;
        private Button? _stopButton;
        
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.activity_session);
            
            _elapsedTemText = FindViewById<TextView>(Resource.Id.elapsed_time);
            _distanceText = FindViewById<TextView>(Resource.Id.distance);
            
            _lapsText = FindViewById<TitledLabelView>(Resource.Id.laps);
            _lapsText.TitleText = "Laps:";
            _lastLapText = FindViewById<TitledLabelView>(Resource.Id.last_lap);
            _lastLapText.TitleText = "Last lap:";
            _bestLapText = FindViewById<TitledLabelView>(Resource.Id.best_lap);
            _bestLapText.TitleText = "Best lap:";
            _lastSectorText = FindViewById<TitledLabelView>(Resource.Id.last_sector);
            _lastSectorText.TitleText = "Last 100m:";
            _bestSectorText = FindViewById<TitledLabelView>(Resource.Id.best_sector);
            _bestSectorText.TitleText = "Best 100m:";

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
            ViewModel = AndroidNavigationService.SharedInstance.Container.GetInstance<LiveSessionViewModel>();
            ViewModel.SetNavigationService(AndroidNavigationService.SharedInstance);
            ViewModel.PropertyChanged+= ViewModelOnPropertyChanged;
            
            UpdateButtonsState();
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModel.TotalTime))
            {
                UpdateTime();
                return;
            }
            
            if (e.PropertyName == nameof(ViewModel.Distance))
            {
                UpdateDistance();
                return;
            }
            
            if (e.PropertyName == nameof(ViewModel.Laps))
            {
                UpdateLaps();
                return;
            }
            
            if (e.PropertyName == nameof(ViewModel.LastLapTime))
            {
                UpdateLastLapTime();
                return;
            }
            if (e.PropertyName == nameof(ViewModel.BestLapTime))
            {
                UpdateBestLapTime();
                return;
            }
            
            if (e.PropertyName == nameof(ViewModel.LastSectorTime))
            {
                UpdateLastSector();
                return;
            }
            if (e.PropertyName == nameof(ViewModel.BestSectorTime))
            {
                UpdateBestSector();
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

        private void UpdateTime()
        {
            if (_elapsedTemText != null) _elapsedTemText.Text = ViewModel?.TotalTime;
        }
        
        private void UpdateLastLapTime()
        {
            if (_lastLapText != null) _lastLapText.ValueText = ViewModel?.LastLapTime;
        }

        private void UpdateDistance()
        {
            if (_distanceText != null) _distanceText.Text = ViewModel?.Distance;
        }

        private void UpdateBestLapTime()
        {
            if (_lastLapText != null) _bestLapText.ValueText = ViewModel?.BestLapTime;
        }

        private void UpdateLaps()
        {
            if (_lapsText != null) _lapsText.ValueText = ViewModel?.Laps;
        }

        private void UpdateLastSector()
        {
            if (_lastSectorText != null) _lastSectorText.ValueText = ViewModel?.LastSectorTime;
        }
        
        private void UpdateBestSector()
        {
            if (_bestSectorText != null) _bestSectorText.ValueText = ViewModel?.BestSectorTime;
        }
    }
}