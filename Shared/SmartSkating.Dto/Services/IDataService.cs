using System.Collections.Generic;
using System.Threading.Tasks;
using Sanet.SmartSkating.Dto.Models;

namespace Sanet.SmartSkating.Dto.Services
{
    public interface IDataService
    {
        Task<bool> SaveWayPointAsync(WayPointDto wayPoint);
        string ErrorMessage { get; }
        Task<List<WayPointDto>> GetAllWayPointsAsync();
        Task<bool> DeleteWayPointAsync(string id);
    }
}