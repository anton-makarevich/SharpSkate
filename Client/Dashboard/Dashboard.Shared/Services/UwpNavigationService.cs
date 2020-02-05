using System.Threading.Tasks;
using Sanet.SmartSkating.Services;
using Sanet.SmartSkating.ViewModels.Base;

namespace Dashboard.Services
{
    public class UwpNavigationService:INavigationService
    {
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
            throw new System.NotImplementedException();
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