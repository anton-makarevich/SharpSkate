using System;
using System.ComponentModel;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.ViewModels;
using Sanet.SmartSkating.ViewModels.Wrappers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Sanet.SmartSkating.Tizen.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SessionsView
    {
        public SessionsView()
        {
            InitializeComponent();
            ConfirmButton.IsEnable = false;
        }

        private void OnTrackSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is SessionDto sessionDto)
            {
                ViewModel?.SelectSession(sessionDto);
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
            if (e.PropertyName == nameof(TracksViewModel.HasSelectedTrack))
                ConfirmButton.IsEnable = ViewModel.CanStart;
        }

        private void ConfirmButtonOnClicked(object sender, EventArgs e)
        {
            ViewModel?.StartCommand?.Execute(null);
        }
    }
}