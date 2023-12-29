using System.IO;
using System.Linq;
using System.Reflection;
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
#else
            "appsettings.json";
#endif
        public ConfigService()
        {
            var assembly = Assembly.GetAssembly(typeof(ConfigService));
            var resourceName = assembly.GetManifestResourceNames()
                .FirstOrDefault(f=> f.Contains(ConfigFile));

            using var stream = assembly.GetManifestResourceStream(resourceName);
            _configuration = new ConfigurationBuilder()
                .AddJsonStream(stream)
                .Build();
        }

        public string BaseUrl => GetConfigValue();
        public string AzureApiSubscriptionKey => GetConfigValue();

        private string GetConfigValue([CallerMemberName] string configName = "")
        {
            return _configuration[configName];
        }
    }
}