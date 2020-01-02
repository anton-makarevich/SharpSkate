using System;
using Sanet.SmartSkating.Models.Location;

namespace Sanet.SmartSkating.Models.EventArgs
{
    public class CoordinateEventArgs:System.EventArgs
    {
        public Coordinate Coordinate { get; }
        public DateTime? Date { get; }

        public CoordinateEventArgs(Coordinate coordinate, DateTime? date = null)
        {
            Coordinate = coordinate;
            Date = date;
        }
    }
}