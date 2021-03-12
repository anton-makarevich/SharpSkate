using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Dto.Services;

namespace Sanet.SmartSkating.Services.Storage
{
    public class JsonStorageService : IDataService
    {
        private const string SmartSkatingFolder = "SmartSkating";
        private const string WayPointsFolder = "WayPoints";
        private const string SessionsFolder = "Sessions";
        private const string BleFolder = "Ble";

        private static string SmartSkatingPath =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), SmartSkatingFolder);
        
        public Task<bool> SaveWayPointAsync(WayPointDto wayPoint)
        {
            return SaveAsync(wayPoint,WayPointsFolder);
        }

        private static Task<bool> SaveAsync<T>(T entity, string folder) where T:EntityBase
        {
            return Task.Run(() =>
            {
                var path = Path.Combine(
                    SmartSkatingPath, folder);
                var file =
                    Path.Combine(path,
                        $"{entity.Id}.json");
                var stringData = JsonConvert.SerializeObject(entity);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                File.WriteAllText(file, stringData);
                return true;
            });
        }

        public string ErrorMessage { get; } = string.Empty;
        public Task<List<WayPointDto>> GetAllWayPointsAsync()
        {
            return GetAllAsync<WayPointDto>(WayPointsFolder);
        }

        public async Task<List<WayPointDto>> GetWayPointForSessionAsync(string sessionId)
        {
            return (await GetAllAsync<WayPointDto>(WayPointsFolder))
                .Where(w => w.SessionId == sessionId).ToList();
        }

        private static Task<List<T>> GetAllAsync<T>(string folder)
        {
            return Task.Run(() =>
            {
                var path = Path.Combine(
                    SmartSkatingPath, folder);
                if (!Directory.Exists(path))
                    return new List<T>();
                var files = Directory.EnumerateFiles(path);

                return files
                    .Where(f => f.EndsWith(".json"))
                    .Select(File.ReadAllText)
                    .Select(JsonConvert.DeserializeObject<T>).ToList();
            });
        }

        public Task<bool> DeleteWayPointAsync(string id)
        {
            return DeleteAsync(id, WayPointsFolder);
        }

        private static Task<bool> DeleteAsync(string id, string folder)
        {
            return Task.Run(() =>
            {
                var path = Path.Combine(
                    SmartSkatingPath, folder);
                if (!Directory.Exists(path))
                    return false;
                var dtoFileName = Path.Combine(path, $"{id}.json");
                if (!File.Exists(dtoFileName))
                    return false;

                File.Delete(dtoFileName);
                return true;
            });
        }

        public Task<bool> SaveSessionAsync(SessionDto session)
        {
            return SaveAsync(session,SessionsFolder);
        }

        public Task<List<SessionDto>> GetAllSessionsAsync()
        {
            return GetAllAsync<SessionDto>(SessionsFolder);
        }

        public Task<bool> DeleteSessionAsync(string id)
        {
            return DeleteAsync(id, SessionsFolder);
        }

        public Task<bool> SaveBleScanAsync(BleScanResultDto bleScanResult)
        {
            return SaveAsync(bleScanResult,BleFolder);
        }

        public Task<List<BleScanResultDto>> GetAllBleScansAsync()
        {
            return GetAllAsync<BleScanResultDto>(BleFolder);
        }

        public Task<bool> DeleteBleScanAsync(string bleScanId)
        {
            return DeleteAsync(bleScanId, BleFolder);
        }

        public Task<bool> SaveDeviceAsync(DeviceDto deviceDto)
        {
            throw new NotImplementedException();
        }

        public Task<List<SessionDto>> GetAllSessionsForAccountAsync(string accountId)
        {
            throw new NotImplementedException();
        }
    }
}