using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sanet.SmartSkating.Models;

namespace Sanet.SmartSkating.Services.Storage
{
    public class JsonStorageService: IStorageService
    {
        private static string DocumentsFolder => 
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SmartSkating");

        public Task SaveCoordinateAsync(Coordinate coordinate)
        {
            return Task.Factory.StartNew(() =>
            {
                var date = DateTime.Now;
                var file = 
                    Path.Combine(
                        DocumentsFolder, 
                        $"{date.Year}-{date.Month}-{date.Day}-{date.Hour}-{date.Minute}-{date.Second}.json");
                var stringData = JsonConvert.SerializeObject(coordinate);
                if (!Directory.Exists(DocumentsFolder))
                    Directory.CreateDirectory(DocumentsFolder);
                File.WriteAllText(file, stringData);
            });
        }
    }
}