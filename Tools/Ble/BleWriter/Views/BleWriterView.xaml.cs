using BleWriter.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BleWriter.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BleWriterView : ContentPage
    {
        public BleWriterView()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is BleWriterViewModel vm)
                vm.AttachHandlers();
        }
    }
}