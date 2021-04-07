using Acr.UserDialogs;
using Android.App;
using Refit;
using Sanet.SmartSkating.Droid.Services.Location;
using Sanet.SmartSkating.Dto;
#if TEST
using Sanet.SmartSkating.Tizen.Services.Location;
using Sanet.SmartSkating.Xf.Droid.Services;
#endif

using Sanet.SmartSkating.Dto.Services;
using Sanet.SmartSkating.Services;
using Sanet.SmartSkating.Services.Account;
using Sanet.SmartSkating.Services.Api;
using Sanet.SmartSkating.Services.Hardware;
using Sanet.SmartSkating.Services.Location;
using Sanet.SmartSkating.Services.Storage;
using Sanet.SmartSkating.Services.Tracking;
using Sanet.SmartSkating.ViewModels;
using Sanet.SmartSkating.Xf.Droid.AndroidShared.Services.Hardware;
using Sanet.SmartSkating.Xf.Droid.DummyServices.Services;
using SimpleInjector;

namespace Sanet.SmartSkating.WearOs
{
    public static class ContainerExtensions
    {
        public static void RegisterModules(this Container container, Activity activity)
        {
            container.RegisterWearOsModule(activity);
        }

        private static void RegisterWearOsModule(this Container container, Activity activity)
        {
            var dataService = new JsonStorageService();
            container.Register<TracksViewModel>();
            container.Register<SessionsViewModel>();
            container.Register<LiveSessionViewModel>();
            container.Register<StartViewModel>();

            #if TEST
            container.RegisterInstance<ILocationService>(new DummyLocationService("Schaatsnaacht", 100));
            var resourceReader = new EmbeddedResourceReader();
            container.RegisterInstance<IBleLocationService>(
                new DummyBleLocationService(
                    resourceReader,
                    new LocalBleDevicesProvider(resourceReader),
                    dataService,
                    0.5)
                );
            container.RegisterSingleton<IBluetoothService,DummyBluetoothService>();
            container.RegisterSingleton<IDataSyncService, DebugSyncService>();
            #else
            container.RegisterInstance<ILocationService>(new LocationManagerService(activity));
            container.RegisterSingleton<IBleLocationService,AndroidBleService>();
            container.RegisterInstance<IBluetoothService>(new AndroidBluetoothService(activity));
            container.RegisterSingleton<IDataSyncService,DataSyncService>();
            #endif

            container.RegisterSingleton<IResourceReader,EmbeddedResourceReader>();
            container.RegisterSingleton<IConnectivityService,EssentialsConnectivityService>();
            container.RegisterInstance(RestService.For<IApiService>(ApiNames.BaseUrl));
            container.RegisterInstance(UserDialogs.Instance);
            container.RegisterSingleton<IDeviceInfo,EssentialsDeviceInfo>();
            container.RegisterSingleton<IPreferences,EssentialsPreferences>();
            container.RegisterSingleton<IAccountService,AccountService>();
            container.RegisterInstance<IDataService>(dataService);
            container.RegisterSingleton<ITrackProvider, LocalTrackProvider>();
            container.RegisterSingleton<IBleDevicesProvider,LocalBleDevicesProvider>();
            container.RegisterSingleton<ITrackService, TrackService>();
            container.RegisterSingleton<ISessionProvider, SessionProvider>();
            container.RegisterSingleton<ISessionManager, SessionManager>();
            container.RegisterSingleton<ISettingsService, SettingsService>();
            container.RegisterSingleton<IDateProvider, DateProvider>();
            container.RegisterSingleton<ISyncService, SignalRService>();
            container.RegisterSingleton<ISessionInfoHelper,SessionInfoHelper>();
        }
    }
}
