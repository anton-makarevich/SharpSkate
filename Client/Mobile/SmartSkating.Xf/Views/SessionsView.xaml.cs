using Sanet.SmartSkating.ViewModels.Wrappers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Sanet.SmartSkating.Xf.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SessionsView 
    {
        public SessionsView()
        {
            InitializeComponent();
        }

        private void OnSessionSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (!(e.SelectedItem is SessionViewModel sessionDto) || ViewModel == null) return;
            ViewModel.SelectedSession = sessionDto;
        }
    }
}