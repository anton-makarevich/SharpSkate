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
        Task<List<WayPointDto>> GetWayPointForSessionAsync(string sessionId);
        Task<bool> DeleteWayPointAsync(string id);

        Task<bool> SaveSessionAsync(SessionDto session);
        Task<List<SessionDto>> GetAllSessionsAsync();
        Task<bool> DeleteSessionAsync(string id);

        Task<bool> SaveBleScanAsync(BleScanResultDto session);
        Task<List<BleScanResultDto>> GetAllBleScansAsync();
        Task<bool> DeleteBleScanAsync(string bleScanId);
        Task<bool> SaveDeviceAsync(DeviceDto deviceDto);
        Task<List<SessionDto>> GetAllSessionsForAccountAsync(string accountId);
    }
}