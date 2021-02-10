using Refit;
using Sanet.SmartSkating.Dto;
using Sanet.SmartSkating.Dto.Services;
using Sanet.SmartSkating.Services;
using Sanet.SmartSkating.Services.Account;
using Sanet.SmartSkating.Services.Api;
using Sanet.SmartSkating.Services.Storage;
using Sanet.SmartSkating.Services.Tracking;
using Sanet.SmartSkating.ViewModels;
using SimpleInjector;

namespace Sanet.SmartSkating.Xf
{
    public static class ContainerExtensions
    {
        public static void RegisterMainModule(this Container container)
        {
            // Register app start viewmodel
            container.Register<StartViewModel>();

            // Register services
            container.RegisterSingleton<IResourceReader,EmbeddedResourceReader>();
            container.RegisterSingleton<ISettingsService, SettingsService>();
            container.RegisterSingleton<IConnectivityService,EssentialsConnectivityService>();
            container.RegisterInstance(RestService.For<IApiService>(ApiNames.BaseUrl));
            container.RegisterSingleton<IDeviceInfo,EssentialsDeviceInfo>();
            container.RegisterSingleton<IPreferences,EssentialsPreferences>();
            container.RegisterSingleton<IAccountService,AccountService>();
            #if TEST
            container.RegisterSingleton<IDataSyncService,DebugSyncService>();
            #else
            container.RegisterSingleton<IDataSyncService,DataSyncService>();
#endif
            container.RegisterSingleton<IDataService, JsonStorageService>();
            container.RegisterSingleton<ITrackProvider,LocalTrackProvider>();
            container.RegisterSingleton<IBleDevicesProvider,LocalBleDevicesProvider>();
            container.RegisterSingleton<ITrackService, TrackService>();
            container.RegisterSingleton<ISessionService,SessionService>();
        }
    }
}
