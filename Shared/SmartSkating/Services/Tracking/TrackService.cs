using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Dto.Services;
using Sanet.SmartSkating.Models.Geometry;
using Sanet.SmartSkating.Models.Location;
using Sanet.SmartSkating.Utils;

namespace Sanet.SmartSkating.Services.Tracking
{
    public class TrackService : ITrackService
    {
        private readonly ITrackProvider _tracksProviderMock;

        public TrackService(ITrackProvider tracksProviderMock)
        {
            _tracksProviderMock = tracksProviderMock;
        }

        public IList<TrackDto> Tracks { get; private set; } = new List<TrackDto>();
        public Rink? SelectedRink { get; private set; }
        public void SelectRinkCloseTo(Coordinate coordinate)
        {
            var track = Tracks
                .Select(t => new
                {
                    Track = t,
                    Distance = (coordinate, new Coordinate(t.Start.Latitude, t.Start.Longitude)).GetRelativeDistance()
                })
                .Where(o => o.Distance <= 0.001)
                .OrderBy(o => o.Distance)
                .Select(o=>o.Track)
                .FirstOrDefault();
            SelectedRink = track!=null
                ? new Rink(new Coordinate(track.Start), new Coordinate(track.Finish), track.Name)
                : null;
        }

        public async Task LoadTracksAsync()
        {
            Tracks = await _tracksProviderMock.GetAllTracksAsync();
        }

        public void SelectRinkByName(string name)
        {
            var track = Tracks.SingleOrDefault(r => r.Name == name);
            if (track == null)
            {
                SelectedRink = null;
                return;
            }
            SelectedRink = !string.IsNullOrEmpty(track.Name) 
                ? new Rink(new Coordinate(track.Start),new Coordinate(track.Finish), track.Name) 
                : null;
        }
    }
}