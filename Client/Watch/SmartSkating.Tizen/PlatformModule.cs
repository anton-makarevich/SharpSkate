using Sanet.SmartSkating.Services.Location;
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
            container.RegisterInstance<ILocationService>(new EssentialsLocationService());
        }
    }
}