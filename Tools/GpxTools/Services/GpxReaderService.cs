using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;
using GpxTools.Models.Gpx;
using Sanet.SmartSkating.Tools.GpxComposer.Models.Gpx;

namespace GpxTools.Services
{
    public class GpxReaderService
    {
        public Task<GpxRoute> ReadEmbeddedGpxFileAsync(string name)
        {
            return Task.Run(() =>
            {
                var assembly = Assembly.GetAssembly(typeof(GpxReaderService));
                var resourceName = assembly.GetManifestResourceNames()
                    .FirstOrDefault(f=> f.ToLower().EndsWith($"{name.ToLower()}.gpx"));
                using var stream = assembly.GetManifestResourceStream(resourceName);
                using var gpxReader = new GpxReader(stream);
                while (gpxReader.Read())
                {
                    if (gpxReader.ObjectType == GpxObjectType.Route)
                    return gpxReader.Route;
                }
                return null;
            }); 
        }
    }
}