using Android.App;
using Sanet.SmartSkating.Droid.Services.Location;
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
            
            container.RegisterInstance<ILocationService>(new LocationManagerService(activity));
            container.RegisterSingleton<IStorageService, JsonStorageService>();
            container.RegisterSingleton<ITrackProvider, LocalTrackProvider>();
            container.RegisterSingleton<ITrackService, TrackService>();
            container.RegisterSingleton<ISessionService, SessionService>();
        }
    }
}