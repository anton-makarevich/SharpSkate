using Acr.UserDialogs;
using Refit;
using Sanet.SmartSkating.Dto;
using Sanet.SmartSkating.Dto.Services;
using Sanet.SmartSkating.Services;
using Sanet.SmartSkating.Services.Account;
using Sanet.SmartSkating.Services.Api;
using Sanet.SmartSkating.Services.Narration;
using Sanet.SmartSkating.Services.Storage;
using Sanet.SmartSkating.Services.Tracking;
using Sanet.SmartSkating.ViewModels;
using Sanet.SmartSkating.Xf.Services;
using SimpleInjector;

namespace Sanet.SmartSkating.Xf
{
    public static class ContainerExtensions
    {
        public static void RegisterMainModule(this Container container)
        {
            var configService = new ConfigService();
            // Register app start viewmodel
            container.Register<StartViewModel>();
            container.Register<TracksViewModel>();
            container.Register<SessionsViewModel>();
            container.Register<LiveSessionViewModel>();

            // Register services
            container.RegisterSingleton<IDateProvider,DateProvider>();
            container.RegisterSingleton<IResourceReader,EmbeddedResourceReader>();
            container.RegisterSingleton<ISettingsService, SettingsService>();
            container.RegisterSingleton<IConnectivityService,EssentialsConnectivityService>();
            container.RegisterInstance(RestService.For<IApiService>(configService.BaseUrl));
            container.RegisterSingleton<IDeviceInfo,EssentialsDeviceInfo>();
            container.RegisterSingleton<IPreferences,EssentialsPreferences>();
            container.RegisterSingleton<IAccountService,AccountService>();
            container.RegisterSingleton<ISessionInfoHelper,SessionInfoHelper>();
            container.RegisterInstance(UserDialogs.Instance);
            container.RegisterSingleton<INarratorService,XamarinEssentialNarratorService>();
            #if TEST
            container.RegisterSingleton<IDataSyncService,DebugSyncService>();
            #else
            container.RegisterSingleton<IDataSyncService,DataSyncService>();
#endif
            container.RegisterSingleton<IDataService, JsonStorageService>();
            container.RegisterSingleton<ITrackProvider,LocalTrackProvider>();
            container.RegisterSingleton<IBleDevicesProvider,LocalBleDevicesProvider>();
            container.RegisterSingleton<ITrackService, TrackService>();
            container.RegisterSingleton<ISessionProvider,SessionProvider>();
            container.RegisterSingleton<ISessionManager,SessionManager>();
            container.RegisterSingleton<ISyncService, SignalRService>();
            container.RegisterInstance<IConfigService>(configService);
        }
    }
}
