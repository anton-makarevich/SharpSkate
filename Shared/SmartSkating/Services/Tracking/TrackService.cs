using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Dto.Services;
using Sanet.SmartSkating.Models;
using Sanet.SmartSkating.Models.Geometry;

namespace Sanet.SmartSkating.Services.Tracking
{
    public class TrackService : ITrackService
    {
        private readonly ITrackProvider _tracksProviderMock;

        public TrackService(ITrackProvider tracksProviderMock)
        {
            _tracksProviderMock = tracksProviderMock;
        }

        public IList<TrackDto> Tracks { get; private set; }
        public Rink SelectedRink { get; private set; }

        public async Task LoadTracksAsync()
        {
            Tracks = await _tracksProviderMock.GetAllTracksAsync();
        }

        public void SelectRinkByName(string name)
        {
            var track = Tracks.SingleOrDefault(r => r.Name == name);
            SelectedRink = !string.IsNullOrEmpty(track.Name) 
                ? new Rink(new Coordinate(track.Start),new Coordinate(track.Finish)) 
                : null;
        }
    }
}