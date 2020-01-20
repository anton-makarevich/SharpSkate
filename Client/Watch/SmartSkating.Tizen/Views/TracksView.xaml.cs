using System;
using System.ComponentModel;
using Sanet.SmartSkating.ViewModels.Wrappers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Sanet.SmartSkating.Xf.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TracksView
    {
        public TracksView()
        {
            InitializeComponent();
            ConfirmButton.IsEnable = false;
        }

        private void OnTrackSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is TrackViewModel trackViewModel)
            {
                ViewModel?.SelectTrack(trackViewModel);
            }
        }

        protected override void OnViewModelSet()
        {
            base.OnViewModelSet();
            if (ViewModel != null) ViewModel.PropertyChanged += ViewModelOnPropertyChanged;
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (ViewModel == null) return;
            if (e.PropertyName == nameof(ViewModel.HasSelectedTrack))
                ConfirmButton.IsEnable = ViewModel.HasSelectedTrack;
        }

        private void ConfirmButtonOnClicked(object sender, EventArgs e)
        {
            ViewModel?.ConfirmSelectionCommand?.Execute(null);
        }
    }
}