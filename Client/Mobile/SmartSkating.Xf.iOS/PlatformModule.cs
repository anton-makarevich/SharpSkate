using Sanet.SmartSkating.Services.Location;
using SimpleInjector;
#if TEST
using Sanet.SmartSkating.Tizen.Services.Location; 
#endif

namespace Sanet.SmartSkating.Xf.Ios
{
    public static class ContainerExtensions
    {
        public static void RegisterModules(this Container container)
        {
            container.RegisterIosModule();
            container.RegisterMainModule();
        }

        private static void RegisterIosModule(this Container container)
        {
#if TEST
            container.RegisterInstance<ILocationService>(new DummyLocationService("Schaatsnaacht", 10));
#else
            container.RegisterSingleton<ILocationService, EssentialsLocationService>();
#endif
        }
    }
}