using System.Collections.Generic;
using System.Threading.Tasks;
using Sanet.SmartSkating.Dto.Models;

namespace Sanet.SmartSkating.Dto.Services
{
    public class LocalTrackProvider : ITrackProvider
    {
        private readonly IResourceReader _resourceReader;

        public LocalTrackProvider(IResourceReader resourceReader)
        {
            _resourceReader = resourceReader;
        }

        public Task<List<TrackDto>> GetAllTracksAsync()
        {
            return _resourceReader.ReadEmbeddedResourceAsync<LocalTrackProvider, TrackDto>("tracks.json");
        }
    }
}