using Android.App;
using Refit;
using Sanet.SmartSkating.Droid.Services.Location;
using Sanet.SmartSkating.Dto;
#if DEBUG
using Sanet.SmartSkating.Tizen.Services.Location;
#else
using Sanet.SmartSkating.Droid.Services.Location;
#endif

using Sanet.SmartSkating.Dto.Services;
using Sanet.SmartSkating.Services.Account;
using Sanet.SmartSkating.Services.Api;
using Sanet.SmartSkating.Services.Location;
using Sanet.SmartSkating.Services.Storage;
using Sanet.SmartSkating.Services.Tracking;
using Sanet.SmartSkating.ViewModels;
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
            container.Register<TracksViewModel>();
            container.Register<LiveSessionViewModel>();
            container.Register<StartViewModel>();
            
            #if DEBUG
            container.RegisterInstance<ILocationService>(new DummyLocationService("Schaatsnaacht", 100));
            #else
            container.RegisterInstance<ILocationService>(new LocationManagerService(activity));
            #endif
            
            container.RegisterSingleton<IConnectivityService,EssentialsConnectivityService>();
            container.RegisterInstance(RestService.For<IApiService>(ApiNames.BaseUrl));
            container.RegisterSingleton<IAccountService,EssentialsAccountService>();
            container.RegisterSingleton<IDataSyncService,DataSyncService>();
            container.RegisterSingleton<IDataService, JsonStorageService>();
            container.RegisterSingleton<ITrackProvider, LocalTrackProvider>();
            container.RegisterSingleton<IBleDevicesProvider,LocalBleDevicesProvider>();
            container.RegisterSingleton<IBleLocationService,AndroidBleService>();
            container.RegisterSingleton<ITrackService, TrackService>();
            container.RegisterSingleton<ISessionService, SessionService>();
        }
    }
}