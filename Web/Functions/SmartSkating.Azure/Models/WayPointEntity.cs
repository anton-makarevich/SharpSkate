using System;
using Microsoft.WindowsAzure.Storage.Table;
using Sanet.SmartSkating.Dto.Models;

namespace Sanet.SmartSkating.Azure.Models
{
    public class WayPointEntity:TableEntity
    {
        public WayPointEntity(WayPointDto wayPoint)
        {
            PartitionKey = wayPoint.SessionId;
            RowKey = wayPoint.Id;

            WayPointType = wayPoint.WayPointType;

            Latitude = wayPoint.Coordinate.Latitude;
            Longitude = wayPoint.Coordinate.Longitude;
            Time = wayPoint.Time;
        }

        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public string WayPointType { get; set; }

        public DateTime Time { get; set; }
    }
}