using System;
using System.Net.Http;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using Sanet.SmartSkating.Dashboard.Avalonia.Services;
using Sanet.SmartSkating.Dashboard.Avalonia.Services.Dummy;
using Sanet.SmartSkating.Dashboard.Avalonia.Views;
using Sanet.SmartSkating.Dto;
using Sanet.SmartSkating.Dto.Services;
using Sanet.SmartSkating.Dto.Services.Account;
using Sanet.SmartSkating.Services;
using Sanet.SmartSkating.Services.Account;
using Sanet.SmartSkating.Services.Api;
using Sanet.SmartSkating.Services.Location;
using Sanet.SmartSkating.Services.Tracking;
using Sanet.SmartSkating.ViewModels;
using MainWindow = Sanet.SmartSkating.Dashboard.Avalonia.Views.MainWindow;

namespace Sanet.SmartSkating.Dashboard.Avalonia
{
    public partial class App : Application
    {
        private IServiceProvider _serviceProvider;

        private readonly IServiceCollection _services;
        private INavigationService _navigationService;

        public App()
        {
            _services = new ServiceCollection();
            ConfigureServices(_services);
        }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            _serviceProvider = _services.BuildServiceProvider();
            var loginView = new LoginView();
            switch (ApplicationLifetime)
            {
                case IClassicDesktopStyleApplicationLifetime desktop:
                    _navigationService = new AvaloniaNavigationService(desktop, _serviceProvider);
                    loginView.ViewModel = _navigationService.GetViewModel<LoginViewModel>();
                    desktop.MainWindow = new MainWindow();
                    desktop.MainWindow.Content = loginView;
                    break;
                case ISingleViewApplicationLifetime singleViewPlatform:
                    var mainWindow = new ContentControl();
                    _navigationService = new AvaloniaSingleViewNavigationService(singleViewPlatform, mainWindow, _serviceProvider);
                    loginView.ViewModel = _navigationService.GetViewModel<LoginViewModel>();               
                    mainWindow.Content = loginView;
                    break;
                default:
                    throw new Exception("Unsupported app type");
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void ConfigureServices(IServiceCollection services)
        {
//#if __WASM__
			// var httpHandler = new WasmHttpHandler();
// #el

            var configService = new ConfigService();
            var httpHandler = new HttpClientHandler();
            var httpClient = new HttpClient(httpHandler,false)
            {
                BaseAddress = new Uri(configService.BaseUrl)
            };
            // Services
            _services
                .AddSingleton<IConfigService>(configService)
                .AddSingleton(RestService.For<IApiService>(httpClient))
                .AddSingleton<ILoginService, LoginService>()
                .AddSingleton<IAccountService, AccountService>()
                .AddSingleton<IDeviceInfo, EssentialsDeviceInfo>()
                .AddSingleton<IPreferences, EssentialsPreferences>()
                .AddSingleton<ISessionProvider, SessionProvider>()
                .AddSingleton<ISettingsService>(new SettingsService {CanInterpolateSectors = true})
                .AddSingleton<ITrackService, TrackService>()
                .AddSingleton<ITrackProvider, LocalTrackProvider>()
                .AddSingleton<IResourceReader, EmbeddedResourceReader>()
                .AddSingleton<ISessionManager, SessionManager>()
                .AddSingleton<ISyncService, SignalRService>()
                .AddSingleton<IDateProvider, DateProvider>()
                .AddSingleton<ISessionInfoHelper, SessionInfoHelper>()
                .AddSingleton<ILocationService, DummyLocationService>()
                .AddSingleton<IDataSyncService, DummyDataSyncService>()
                .AddSingleton<IBleLocationService, DummyBleService>()
                .AddSingleton<Acr.UserDialogs.IUserDialogs, DummyUserDialogs>()

                // ViewModels
                .AddScoped<LoginViewModel, LoginViewModel>()
                .AddScoped<SessionsViewModel, SessionsViewModel>()
                .AddTransient<SessionDetailsViewModel, SessionDetailsViewModel>();
        }
    }
}
