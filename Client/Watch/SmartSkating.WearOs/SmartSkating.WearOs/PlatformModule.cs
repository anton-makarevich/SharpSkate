using Android.App;
#if DEBUG
using Sanet.SmartSkating.Tizen.Services;
#else
using Sanet.SmartSkating.Droid.Services.Location;
#endif

using Sanet.SmartSkating.Dto.Services;
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
            container.RegisterSingleton<ILocationService,DummyLocationService>();
            #else
            container.RegisterInstance<ILocationService>(new LocationManagerService(activity));
            #endif
            container.RegisterSingleton<IStorageService, JsonStorageService>();
            container.RegisterSingleton<ITrackProvider, LocalTrackProvider>();
            container.RegisterSingleton<ITrackService, TrackService>();
            container.RegisterSingleton<ISessionService, SessionService>();
        }
    }
}