using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Configuration;

namespace Sanet.SmartSkating.Dto.Services
{
    public class ConfigService:IConfigService
    {
        private readonly IConfiguration _configuration;

        private const string ConfigFile =
#if DEBUG
         "appsettings.debug.json";
#endif
            "appsettings.json";
        public ConfigService()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(ConfigFile, optional: false, reloadOnChange: true)
                .Build();
        }

        public string BaseUrl => GetConfigValue();// "https://smartskating.azure-api.net/smartskating";
        public string AzureApiSubscriptionKey => GetConfigValue();//"<AzureApiKey>"

        private string GetConfigValue([CallerMemberName] string configName = "")
        {
            return _configuration[configName];
        }
    }
}