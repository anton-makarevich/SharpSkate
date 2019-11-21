using Sanet.SmartSkating.ViewModels.Base;
using Sanet.SmartSkating.Views;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace Sanet.SmartSkating.Xf.Views.Base
{
    public abstract class BaseView<TViewModel> : ContentPage, IBaseView<TViewModel> where TViewModel : BaseViewModel
    {
        protected bool NavigationBarEnabled = false;

        private TViewModel _viewModel;

        protected BaseView()
        {
            Xamarin.Forms.NavigationPage.SetHasNavigationBar(this, false);
            BackgroundColor = Color.Black;

            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
        }

        public virtual TViewModel ViewModel
        {
            get => _viewModel; 
            set
            {
                _viewModel = value;
                BindingContext = _viewModel;
                OnViewModelSet();
            }
        }

        object IBaseView.ViewModel
        {
            get => _viewModel;
            set => ViewModel = (TViewModel)value;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel.AttachHandlers();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            ViewModel.DetachHandlers();
        }

        protected virtual void OnViewModelSet() { }
    }
}
