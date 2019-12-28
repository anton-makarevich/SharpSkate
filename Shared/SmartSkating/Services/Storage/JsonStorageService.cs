using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Dto.Services;
using Sanet.SmartSkating.Models;
using Sanet.SmartSkating.Models.Location;

namespace Sanet.SmartSkating.Services.Storage
{
    public class JsonStorageService : IDataService
    {
        private static string SmartSkatingFolder =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SmartSkating");

        public Task<List<Coordinate>> LoadAllCoordinatesAsync()
        {
            if (!Directory.Exists(SmartSkatingFolder))
                return Task.FromResult<List<Coordinate>>(new List<Coordinate>());
            return Task<List<Coordinate>>.Factory.StartNew(() =>
            {
                var coordinates = new List<Coordinate>();
                
                var files = Directory.EnumerateFiles(SmartSkatingFolder);
                foreach (var file in files)
                {
                    var stringData = File.ReadAllText(file);
                    #if DEBUG
                    Console.WriteLine($"{file}: - {stringData}");
                    #endif
                    coordinates.Add(JsonConvert.DeserializeObject<Coordinate>(stringData));
                }

                return coordinates;
            });
        }

        public Task<bool> SaveWayPointAsync(WayPointDto wayPoint)
        {
            return Task.Run(() =>
            {
                var date = DateTime.Now;
                var file =
                    Path.Combine(
                        SmartSkatingFolder,
                        $"{date.Year}-{date.Month}-{date.Day}-{date.Hour}-{date.Minute}-{date.Second}.json");
                var stringData = JsonConvert.SerializeObject(wayPoint);
                if (!Directory.Exists(SmartSkatingFolder))
                    Directory.CreateDirectory(SmartSkatingFolder);
                File.WriteAllText(file, stringData);
                return true;
            });
        }

        public string ErrorMessage { get; }
    }
}