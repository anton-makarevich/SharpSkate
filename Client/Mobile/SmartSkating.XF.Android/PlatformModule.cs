using Android.App;
using Sanet.SmartSkating.Services.Location;
using Sanet.SmartSkating.Xf.Droid.Services.Location;
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
            container.RegisterInstance<ILocationService>(new LocationManagerService(activity));
        }
    }
}