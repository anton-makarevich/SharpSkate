using System.IO;
using System.Reflection;

namespace Sanet.SmartSkating.Tools.GpxComposer.Models
{
    public class GpxComposer
    {
        private Errors ReadCoordinates()
        {
            var path = $"{Assembly.GetCallingAssembly().Location}Coordinates";

            if (Directory.Exists(path))
                return Errors.NoCoordinateFiles;
            return Errors.Ok;
        }
    }
}