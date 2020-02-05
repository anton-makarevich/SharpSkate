using Windows.UI.Xaml.Controls;
using Sanet.SmartSkating.ViewModels.Base;
using Sanet.SmartSkating.Views;

namespace Sanet.SmartSkating.Dashboard.Views.Base
{
    public abstract class BasePage<TViewModel> :Page, IBaseView<TViewModel> where TViewModel : BaseViewModel
    {
        protected BasePage()
        {
            ViewModel = App.Container.GetInstance<TViewModel>();
        }
        
        private TViewModel? _viewModel;
        
        public TViewModel? ViewModel
        {
            get => _viewModel;
            private set
            {
                _viewModel = value;
                DataContext = _viewModel;
                OnViewModelSet();
            }
        }

        object? IBaseView.ViewModel
        {
            get => _viewModel;
            set => ViewModel = (TViewModel?)value;
        }
        protected void OnViewModelSet() { }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            ViewModel?.AttachHandlers();
        }

        protected override void OnUnloaded()
        {
            base.OnUnloaded();
            ViewModel?.DetachHandlers();
        }
    }
}