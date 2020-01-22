using Sanet.SmartSkating.Services.Hardware;
using Sanet.SmartSkating.Services.Location;
using Sanet.SmartSkating.Tizen.Services.Hardware;
using Sanet.SmartSkating.Tizen.Services.Location;
using Sanet.SmartSkating.Xf;
using SimpleInjector;

namespace Sanet.SmartSkating.Tizen
{
    public static class ContainerExtensions
    {
        public static void RegisterModules(this Container container)
        {
            container.RegisterTizenModule();
            container.RegisterMainModule();
        }

        private static void RegisterTizenModule(this Container container)
        {
            #if DEBUG
            container.RegisterInstance<ILocationService>(new DummyLocationService("Schaatsnaacht", 100));
            #else
            container.RegisterSingleton<ILocationService,EssentialsLocationService>();
#endif
            container.RegisterSingleton<IBleLocationService,TizenBleService>();
            container.RegisterSingleton<IBluetoothService,TizenBluetoothService>();
        }
    }
}