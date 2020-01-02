using Sanet.SmartSkating.Services.Location;
using SimpleInjector;
#if DEBUG
using Sanet.SmartSkating.Tizen.Services; 
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
#if DEBUG
            container.RegisterInstance<ILocationService>(new DummyLocationService("Schaatsnaacht", 0));
#else
            container.RegisterSingleton<ILocationService, EssentialsLocationService>();
#endif
        }
    }
}