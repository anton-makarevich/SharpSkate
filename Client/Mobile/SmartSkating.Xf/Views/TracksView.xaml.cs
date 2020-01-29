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
        }

        private void OnTrackSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is TrackViewModel trackViewModel)
                ViewModel?.SelectTrack(trackViewModel);
        }
    }
}