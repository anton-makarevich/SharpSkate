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
        private static string SmartSkatingFolder =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SmartSkating");
        
        public Task<bool> SaveWayPointAsync(WayPointDto wayPoint)
        {
            return Task.Run(() =>
            {
                var file =
                    Path.Combine(
                        SmartSkatingFolder,
                        $"{wayPoint.Id}.json");
                var stringData = JsonConvert.SerializeObject(wayPoint);
                if (!Directory.Exists(SmartSkatingFolder))
                    Directory.CreateDirectory(SmartSkatingFolder);
                File.WriteAllText(file, stringData);
                return true;
            });
        }

        public string ErrorMessage { get; } = string.Empty;
        public Task<List<WayPointDto>> GetAllWayPointsAsync()
        {
            if (!Directory.Exists(SmartSkatingFolder))
                return Task.FromResult(new List<WayPointDto>());
            return Task.Run(() =>
            {
                var files = Directory.EnumerateFiles(SmartSkatingFolder);

                return files
                    .Where(f => f.EndsWith(".json"))
                    .Select(File.ReadAllText)
                    .Select(JsonConvert.DeserializeObject<WayPointDto>).ToList();
            });
        }

        public Task<bool> DeleteWayPointAsync(string id)
        {

            return Task.Run(() =>
            {
                if (!Directory.Exists(SmartSkatingFolder))
                    return false;
                var dtoFileName = Path.Combine(SmartSkatingFolder, $"{id}.json");
                if (!File.Exists(dtoFileName))
                    return false;
                
                File.Delete(dtoFileName);
                return true;
            });
        }
    }
}