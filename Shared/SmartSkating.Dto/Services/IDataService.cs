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
        
        Task<bool> SaveSessionAsync(SessionDto session);
        Task<List<SessionDto>> GetAllSessionsAsync();
        Task<bool> DeleteSessionAsync(string id);
        
        Task<bool> SaveBleAsync(BleScanResultDto session);
    }
}