using Sanet.SmartSkating.Services;
using Sanet.SmartSkating.ViewModels.Base;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace DashboardUno.Shared.Services
{
    class UwpNavigationService : INavigationService
    {
        private readonly Frame _rootFrame;
        private readonly IServiceProvider _container;

        public UwpNavigationService(Frame rootFrame, IServiceProvider container)
        {
            _rootFrame = rootFrame;
            _container = container;
        }

        public Task CloseAsync()
        {
            throw new NotImplementedException();
        }

        public T GetNewViewModel<T>() where T : BaseViewModel
        {
            throw new NotImplementedException();
        }

        public T GetViewModel<T>() where T : BaseViewModel
        {
            throw new NotImplementedException();
        }

        public bool HasViewModel<T>() where T : BaseViewModel
        {
            throw new NotImplementedException();
        }

        public Task NavigateBackAsync()
        {
            throw new NotImplementedException();
        }

        public Task NavigateToRootAsync()
        {
            throw new NotImplementedException();
        }

        public Task NavigateToViewModelAsync<T>(T viewModel) where T : BaseViewModel
        {
            throw new NotImplementedException();
        }

        public Task NavigateToViewModelAsync<T>() where T : BaseViewModel
        {
            throw new NotImplementedException();
        }

        public Task ShowViewModelAsync<T>(T viewModel) where T : BaseViewModel
        {
            throw new NotImplementedException();
        }

        public Task ShowViewModelAsync<T>() where T : BaseViewModel
        {
            throw new NotImplementedException();
        }

        public Task<TResult> ShowViewModelForResultAsync<T, TResult>(T viewModel)
            where T : BaseViewModel
            where TResult : class
        {
            throw new NotImplementedException();
        }

        public Task<TResult> ShowViewModelForResultAsync<T, TResult>()
            where T : BaseViewModel
            where TResult : class
        {
            throw new NotImplementedException();
        }
    }
}
