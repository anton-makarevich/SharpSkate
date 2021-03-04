using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Sanet.SmartSkating.Backend.Azure.Services;
using Sanet.SmartSkating.Dto.Services;
using Sanet.SmartSkating.Dto.Services.Account;

[assembly: FunctionsStartup(typeof(Sanet.SmartSkating.Backend.Functions.Startup))]

namespace Sanet.SmartSkating.Backend.Functions
{
    public class Startup:FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services
                .AddSingleton<IDataService, AzureTablesDataService>()
                .AddSingleton<ILoginService, AzureLoginService>();
        }
    }
}