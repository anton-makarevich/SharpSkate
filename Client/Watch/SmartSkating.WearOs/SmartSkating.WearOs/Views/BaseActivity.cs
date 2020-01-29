using Android.Support.Wearable.Activity;
using Sanet.SmartSkating.ViewModels.Base;
using Sanet.SmartSkating.Views;

namespace Sanet.SmartSkating.WearOs.Views
{
    public abstract class BaseActivity<TViewModel> : WearableActivity,IBaseView<TViewModel> where TViewModel : BaseViewModel
    {
        private TViewModel? _viewModel;
        
        public virtual TViewModel? ViewModel
        {
            get => _viewModel;
            protected set
            {
                _viewModel = value;
                OnViewModelSet();
            }
        }

        object? IBaseView.ViewModel
        {
            get => _viewModel;
            set => ViewModel = (TViewModel?)value;
        }

        protected override void OnStart()
        {
            base.OnStart();
            ViewModel?.AttachHandlers();
        }

        protected override void OnStop()
        {
            base.OnStop();
            ViewModel?.DetachHandlers();
        }

        protected virtual void OnViewModelSet() { }
    }
}
