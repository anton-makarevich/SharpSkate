using Android.App;
using Sanet.SmartSkating.Dto.Services;
using Sanet.SmartSkating.Services.Hardware;
using Sanet.SmartSkating.Services.Location;
using Sanet.SmartSkating.Xf.Droid.DummyServices.Services;
using Sanet.SmartSkating.Xf.Droid.Services;
#if DEBUG
using Sanet.SmartSkating.Tizen.Services.Location;
#else
using Sanet.SmartSkating.Xf.Droid.AndroidShared.Services.Hardware;
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
            container.RegisterInstance<ILocationService>(new DummyLocationService("Schaatsnaacht", 100));
            var resourceReader = new EmbeddedResourceReader();
            container.RegisterInstance<IBleLocationService>(
                new DummyBleLocationService(
                    resourceReader, 
                    new LocalBleDevicesProvider(resourceReader),
                    1)
            );
            container.RegisterSingleton<IBluetoothService,DummyBluetoothService>();
#else
            container.RegisterInstance<ILocationService>(new LocationManagerService(activity));
            container.RegisterSingleton<IBleLocationService,AndroidBleService>();
            container.RegisterInstance<IBluetoothService>(new AndroidBluetoothService(activity));
#endif
        }
    }
}