using Android.App;
using Sanet.SmartSkating.Droid.Services.Location;
using Sanet.SmartSkating.Services.Location;
#if DEBUG
using Sanet.SmartSkating.Tizen.Services;
#else
using Sanet.SmartSkating.Droid.Services.Location;
#endif

using SimpleInjector;

namespace Sanet.SmartSkating.Xf.Droid
{
    public static class ContainerExtensions
    {
        public static void RegisterModules(this Container container, Activity activity)
        {
            container.RegisterAndroidModule(activity);
            container.RegisterMainModule();
        }

        private static void RegisterAndroidModule(this Container container, Activity activity)
        {
#if DEBUG
            container.RegisterInstance<ILocationService>(new DummyLocationService("Schaatsnaacht", 1000));
#else
            container.RegisterInstance<ILocationService>(new LocationManagerService(activity));
#endif
            container.RegisterSingleton<IBleLocationService,AndroidBleService>();
        }
    }
}