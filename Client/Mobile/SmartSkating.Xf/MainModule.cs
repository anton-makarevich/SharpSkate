using Sanet.SmartSkating.Services.Storage;
using Sanet.SmartSkating.ViewModels;
using SimpleInjector;

namespace Sanet.SmartSkating.Xf
{
    public static class ContainerExtensions
    {
        public static void RegisterMainModule(this Container container)
        {
            // Register app start viewmodel
            container.Register<LiveSessionViewModel>();
            
            // Register services
            container.RegisterSingleton<IStorageService, JsonStorageService>();
        }
    }
}