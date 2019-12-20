using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Sanet.SmartSkating.Services;
using Sanet.SmartSkating.ViewModels.Base;

namespace Sanet.SmartSkating.WearOs.Services
{
    public class AndroidNavigationService:INavigationService
    {
        private readonly Activity _activity;

        public AndroidNavigationService(Activity activity)
        {
            _activity = activity;
        }
        
        public T GetNewViewModel<T>() where T : BaseViewModel
        {
            throw new System.NotImplementedException();
        }

        public T GetViewModel<T>() where T : BaseViewModel
        {
            throw new System.NotImplementedException();
        }

        public bool HasViewModel<T>() where T : BaseViewModel
        {
            throw new System.NotImplementedException();
        }

        public Task NavigateToViewModelAsync<T>(T viewModel) where T : BaseViewModel
        {
            return Task.Run(() =>
            {
                var intent = new Intent(_activity, typeof(MainActivity));
                _activity.StartActivity(intent);
            }); 
        }

        public Task NavigateToViewModelAsync<T>() where T : BaseViewModel
        {
            throw new System.NotImplementedException();
        }

        public Task ShowViewModelAsync<T>(T viewModel) where T : BaseViewModel
        {
            throw new System.NotImplementedException();
        }

        public Task ShowViewModelAsync<T>() where T : BaseViewModel
        {
            throw new System.NotImplementedException();
        }

        public Task<TResult> ShowViewModelForResultAsync<T, TResult>(T viewModel) where T : BaseViewModel where TResult : class
        {
            throw new System.NotImplementedException();
        }

        public Task<TResult> ShowViewModelForResultAsync<T, TResult>() where T : BaseViewModel where TResult : class
        {
            throw new System.NotImplementedException();
        }

        public Task NavigateBackAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task CloseAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task NavigateToRootAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}