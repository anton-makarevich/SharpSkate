using System.Collections.Generic;
using System.Threading.Tasks;
using Sanet.SmartSkating.Dto.Models;

namespace Sanet.SmartSkating.Dto.Services
{
    public interface ITrackProvider
    {
        Task<List<TrackDto>> GetAllTracksAsync();
    }
}