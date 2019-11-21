using System;

namespace Sanet.SmartSkating.Models.Location
{
    public struct WayPoint
    {
        public Coordinate Coordinate { get; }
        public DateTime Date { get; }

        public WayPoint(Coordinate coordinate, DateTime date)
        {
            Coordinate = coordinate;
            Date = date;
        }
    }
}