using Sanet.SmartSkating.Services;
using Sanet.SmartSkating.ViewModels;
using Sanet.SmartSkating.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Sanet.SmartSkating.Dashboard.Views;
using Windows.UI.Core;

namespace Sanet.SmartSkating.Dashboard.Services
{
    class UwpNavigationService : INavigationService
    {
        private readonly List<BaseViewModel> _viewModels = new List<BaseViewModel>();
        private readonly Dictionary<Type, Type> _viewModelViewDictionary = new Dictionary<Type, Type>();

        private readonly Frame _rootFrame;
        private readonly IServiceProvider _container;

        public UwpNavigationService(Frame rootFrame, IServiceProvider container)
        {
            _rootFrame = rootFrame;
            _container = container;

            RegisterViewModels();
        }

        private void RegisterViewModels()
        {
            //For now just manual registration
            _viewModelViewDictionary.Add(typeof(LoginViewModel), typeof(LoginView));
            _viewModelViewDictionary.Add(typeof(SessionsViewModel), typeof(SessionsView));
            _viewModelViewDictionary.Add(typeof(SessionDetailsViewModel), typeof(SessionDetailsView));
        }

        private T CreateViewModel<T>() where T : BaseViewModel
        {
            var vm = _container.GetService(typeof(T)) as T;
            vm?.SetNavigationService(this);
            _viewModels.Add(vm);
            return vm;
        }

        private Task OpenViewModelAsync<T>(T viewModel, bool modalPresentation = false)
            where T : BaseViewModel
        {
            return _rootFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                var viewType = CreateView(viewModel);
                {
                    _rootFrame.Navigate(viewType);
                }
            }).AsTask();
        }

        private Type CreateView(BaseViewModel viewModel)
        {
            var viewModelType = viewModel.GetType();

            return _viewModelViewDictionary[viewModelType];
        }

        public Task CloseAsync()
        {
            throw new NotImplementedException();
        }

        public T GetNewViewModel<T>() where T : BaseViewModel
        {
            var vm = (T)_viewModels.FirstOrDefault(f => f is T);

            if (vm != null)
            {
                _viewModels.Remove(vm);
            }

            vm = CreateViewModel<T>();
            _viewModels.Add(vm);
            return vm;
        }

        public T GetViewModel<T>() where T : BaseViewModel
        {
            var vm = (T)_viewModels.FirstOrDefault(f => f is T);
            return vm;
        }

        public bool HasViewModel<T>() where T : BaseViewModel
        {
            var vm = (T)_viewModels.FirstOrDefault(f => f is T);
            return (vm != null);
        }

        public Task NavigateBackAsync()
        {
            return Task.Run(() =>
            {
                _rootFrame.GoBack();
            });
        }

        public Task NavigateToRootAsync()
        {
            throw new NotImplementedException();
        }

        public Task NavigateToViewModelAsync<T>(T viewModel) where T : BaseViewModel
        {
            return OpenViewModelAsync(viewModel);
        }

        public Task NavigateToViewModelAsync<T>() where T : BaseViewModel
        {
            var vm = GetViewModel<T>();
            if (vm == null)
                vm = CreateViewModel<T>();
            return OpenViewModelAsync(vm);
        }

        public Task ShowViewModelAsync<T>(T viewModel) where T : BaseViewModel
        {
            return NavigateToViewModelAsync(viewModel);
        }

        public Task ShowViewModelAsync<T>() where T : BaseViewModel
        {
            return NavigateToViewModelAsync<T>();
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
