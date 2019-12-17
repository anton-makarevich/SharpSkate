using Sanet.SmartSkating.Dto.Services;
using Sanet.SmartSkating.Services.Storage;
using Sanet.SmartSkating.Services.Tracking;
using Sanet.SmartSkating.ViewModels;
using SimpleInjector;

namespace Sanet.SmartSkating.Xf
{
    public static class ContainerExtensions
    {
        public static void RegisterMainModule(this Container container)
        {
            // Register app start viewmodel
            container.Register<TracksViewModel>();
            
            // Register services
            container.RegisterSingleton<IStorageService, JsonStorageService>();
            container.RegisterSingleton<ITrackProvider,LocalTrackProvider>();
            container.RegisterSingleton<ITrackService, TrackService>();
        }
    }
}