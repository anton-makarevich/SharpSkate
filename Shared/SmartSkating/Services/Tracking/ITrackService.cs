using System.Collections.Generic;
using System.Threading.Tasks;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Models;
using Sanet.SmartSkating.Models.Geometry;
using Sanet.SmartSkating.Models.Location;

namespace Sanet.SmartSkating.Services.Tracking
{
    public interface ITrackService
    {
        IList<TrackDto> Tracks { get; }
        Task LoadTracksAsync();
        void SelectRinkByName(string name);
        Rink? SelectedRink { get; }
        void SelectRinkCloseTo(Coordinate locationStub);
    }
}