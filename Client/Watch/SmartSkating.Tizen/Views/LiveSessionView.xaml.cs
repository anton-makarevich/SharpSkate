using System;
using System.ComponentModel;
using Xamarin.Forms.Xaml;

namespace Sanet.SmartSkating.Xf.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LiveSessionView
    {
        public LiveSessionView()
        {
            InitializeComponent();
        }

        protected override void OnViewModelSet()
        {
            base.OnViewModelSet();
            ViewModel.PropertyChanged += ViewModelOnPropertyChanged;
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModel.IsRunning))
            {
                ActionButton.Text = ViewModel.IsRunning ? "Stop" : "Start";
            }
        }

        private void ActionButtonOnClicked(object sender, EventArgs e)
        {
            if (ViewModel.IsRunning)
                ViewModel.StopCommand.Execute(null);
            else
                ViewModel.StartCommand.Execute(null);
        }
    }
}