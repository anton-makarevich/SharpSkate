using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GpxTools.Models.Gpx;
using Newtonsoft.Json;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Tools.GpxComposer.Models.Gpx;

namespace Sanet.SmartSkating.Tools.GpxComposer.Models
{
    public class CoordinatesReader
    {
        private const string Path = "data/user/0/by.sanet.smartskating.wearos/files/SmartSkating/";

        private List<WayPointDto> _wayPoints;
        
        public void ReadFromLog()
        {
            _wayPoints = new List<WayPointDto>();
            var data = File.ReadAllLines("/Users/amakarevich/OneDrive/SmartSkating/skatingData/vera14122019-grefrath/data.log");
            foreach (var line in data)
            {
                if (line.Contains(Path))
                {
                    ParseLine(line);
                }
            }
            WriteGpx();
        }

        public void ReadFromBackup()
        {
            _wayPoints = new List<WayPointDto>();
            var files = Directory.EnumerateFiles(
                "/Users/amakarevich/OneDrive/SmartSkating/skatingData/antonShaatsnaacht2019/rawdata");
            foreach (var file in files)
            {
                var dateCoordinateString = $"{System.IO.Path.GetFileName(file)}: - {File.ReadAllText(file)}";
                ParseDateCoordinateString(dateCoordinateString);
            }
            WriteGpx();
        }

        public void ReadBleFromBackup()
        {
            var bleEvents = new List<BleScanResultDto>();
            var files = Directory.EnumerateFiles(
                "/Users/amakarevich/OneDrive/SmartSkating/skatingData/anton011120/Ble");
            
            foreach (var file in files.Where(f=>!f.Contains(".DS")))
            {
                var dataString = File.ReadAllText(file);

                try
                {
                    bleEvents.Add(JsonConvert.DeserializeObject<BleScanResultDto>(dataString));
                }
                catch (Exception e)
                {
                    var t = e.Message;
                }
            }
            foreach (var group in bleEvents.GroupBy(f=>f.DeviceAddress))
            {
                Console.WriteLine($"Device {group.Key}");
                foreach (var bleEvent in group.OrderBy(b=>b.Time))
                {
                    Console.WriteLine($"{bleEvent.Time.Hour}:{bleEvent.Time.Minute}:{bleEvent.Time.Second} , {bleEvent.Rssi}  , {bleEvent.DeviceAddress}");
                }
                Console.WriteLine("-------");
            }
        }

        private void WriteGpx()
        {
            var t = _wayPoints.OrderBy(f => f.Time);

            var gpxRoute = new GpxRoute();

            foreach (var wayPoint in t)
            {
                //                var startDate = new DateTime(2019,11,16,8,40,0);
                //                var endDate = new DateTime(2019,11,16,9,30,0);
                // if (wayPoint.Date.Day != 23)
                //     continue;

                var gpxPoint = new GpxRoutePoint
                {
                    Latitude = wayPoint.Coordinate.Latitude,
                    Longitude = wayPoint.Coordinate.Longitude,
                    Time = wayPoint.Time
                };

                gpxRoute.RoutePoints.Add(gpxPoint);
            }

            var metaData = new GpxMetadata {Name = "Anton schaatsnaacht 27-12-2019"};

            using var stream = File.OpenWrite("/Users/amakarevich/OneDrive/SmartSkating/skatingData/antonShaatsnaacht2019/schaatsnaacht.gpx");
            using GpxWriter writer = new GpxWriter(stream);
            writer.WriteMetadata(metaData);

            writer.WriteRoute(gpxRoute);
        }


        private void ParseLine(string line)
        {
            var parts = line.Split(Path, StringSplitOptions.RemoveEmptyEntries);
            var dateCoordinate = parts.Last();
            if (!string.IsNullOrEmpty(dateCoordinate))
            {
                ParseDateCoordinateString(dateCoordinate);
            }
        }

        private void ParseDateCoordinateString(string dateCoordinate)
        {
            var dateCoordinateParts = dateCoordinate.Split(".json: - ");
            if (dateCoordinateParts.Length == 2)
            {
                var dateString = dateCoordinateParts[0];
                var coordinateString = dateCoordinateParts[1];
                var date = DateFromString(dateString);
                var coordinate = JsonConvert.DeserializeObject<CoordinateDto>(coordinateString, 
                    new JsonSerializerSettings { FloatParseHandling = FloatParseHandling.Double });
                _wayPoints.Add(new WayPointDto
                {
                    Coordinate = coordinate,
                    Time = date
                });
            }
        }

        private DateTime DateFromString(string dateString)
        {
            var dateComponents = dateString.Split('-');
            if (dateComponents.Length != 6) throw new ArgumentException($"Not a valid date string {dateString}");
            var year = int.Parse(dateComponents[0]);
            var month = int.Parse(dateComponents[1]);
            var day = int.Parse(dateComponents[2]);
            var hour = int.Parse(dateComponents[3]);
            var minute = int.Parse(dateComponents[4]);
            var second = int.Parse(dateComponents[5]);
            return new DateTime(year,month,day,hour,minute,second);
        }
    }
}