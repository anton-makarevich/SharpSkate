using Sanet.SmartSkating.Services.Location;
using SimpleInjector;

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
            container.RegisterSingleton<ILocationService, EssentialsLocationService>();
        }
    }
}