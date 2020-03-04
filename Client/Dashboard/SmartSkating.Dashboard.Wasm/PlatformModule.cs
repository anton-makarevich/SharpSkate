using Refit;
using Sanet.SmartSkating.Dto;
using Sanet.SmartSkating.Dto.Services.Account;
using Sanet.SmartSkating.Services.Account;
using Sanet.SmartSkating.Services.Api;
using Sanet.SmartSkating.ViewModels;
using SimpleInjector;

namespace Sanet.SmartSkating.Dashboard.Wasm
{
    public static class ContainerExtensions
    {
        public static void RegisterUnoMainModule(this Container container)
        {
            // Register app start viewmodel
            container.Register<LoginViewModel>();
            
            // Register services
            container.RegisterSingleton<ILoginService,LoginService>();
            container.RegisterInstance(RestService.For<IApiService>(ApiNames.BaseUrl));
        }
    }
}