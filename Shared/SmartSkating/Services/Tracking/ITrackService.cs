using System.Collections.Generic;
using System.Threading.Tasks;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Models.Geometry;

namespace Sanet.SmartSkating.Services.Tracking
{
    public interface ITrackService
    {
        IList<TrackDto> Tracks { get; }
        Task LoadTracksAsync();
        Rink? GetRinkByName(string name);
    }
}