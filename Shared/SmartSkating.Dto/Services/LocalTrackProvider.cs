using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sanet.SmartSkating.Dto.Models;

namespace Sanet.SmartSkating.Dto.Services
{
    public class LocalTrackProvider : ITrackProvider
    {
        public Task<List<TrackDto>> GetAllTracksAsync()
        {
            return Task.Run(() =>
            {
                var assembly = Assembly.GetAssembly(typeof(LocalTrackProvider));
                var resourceName = assembly.GetManifestResourceNames()
                    .FirstOrDefault(f=> f.ToLower().EndsWith("tracks.json"));
                using var stream = assembly.GetManifestResourceStream(resourceName);
                using var reader = new StreamReader(stream 
                                                    ?? throw new MissingManifestResourceException(
                                                        $"Missing local tracks file"));
                var result = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<List<TrackDto>>(result);
            }); 
        }
    }
}