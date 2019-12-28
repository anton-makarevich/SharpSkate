using System;
using Sanet.SmartSkating.Models.Location;

namespace Sanet.SmartSkating.Models.Training
{
    public struct WayPoint
    {
        public Coordinate OriginalCoordinate { get; }
        public Coordinate AdjustedCoordinate { get; }
        public WayPointTypes Type { get; }
        public DateTime Date { get; }
        
        public WayPoint(Coordinate coordinate, DateTime date)
            :this(coordinate,coordinate,date,WayPointTypes.Unknown)
        {
        }

        public WayPoint(
            Coordinate originalCoordinate, 
            Coordinate adjustedCoordinate,
            DateTime date,
            WayPointTypes type)
        {
            OriginalCoordinate = originalCoordinate;
            AdjustedCoordinate = adjustedCoordinate;
            Date = date;
            Type = type;
        }
    }
}