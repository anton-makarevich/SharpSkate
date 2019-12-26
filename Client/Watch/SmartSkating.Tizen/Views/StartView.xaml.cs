using System;
using System.ComponentModel;
using Xamarin.Forms.Xaml;

namespace Sanet.SmartSkating.Xf.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StartView
    {
        public StartView()
        {
            InitializeComponent();
            StartButton.IsEnable = false;
        }

        protected override void OnViewModelSet()
        {
            base.OnViewModelSet();
            ViewModel.PropertyChanged += ViewModelOnPropertyChanged;
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModel.CanStart))
                StartButton.IsEnable = ViewModel.CanStart;
        }

        private void StartButtonOnClicked(object sender, EventArgs e)
        {
            ViewModel.StartCommand.Execute(null);
        }
    }
}