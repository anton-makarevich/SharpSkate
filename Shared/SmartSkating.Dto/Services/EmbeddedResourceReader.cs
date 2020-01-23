using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Sanet.SmartSkating.Dto.Services
{
    public class EmbeddedResourceReader : IResourceReader
    {
        public Task<List<TR>> ReadEmbeddedResourceAsync<T, TR>(string filename, string errorMessage)
        {
            return Task.Run(() =>
            {
                var assembly = Assembly.GetAssembly(typeof(T));
                var resourceName = assembly.GetManifestResourceNames()
                    .FirstOrDefault(f=> f.ToLower().EndsWith(filename));
                using var stream = assembly.GetManifestResourceStream(resourceName);
                using var reader = new StreamReader(stream 
                                                    ?? throw new MissingManifestResourceException(
                                                        errorMessage));
                var result = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<List<TR>>(result);
            }); 
        }
    }
}