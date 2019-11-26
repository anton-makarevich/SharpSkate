using System;
using System.Collections.Generic;
using System.Linq;
using Sanet.SmartSkating.Models.Location;

namespace Sanet.SmartSkating.Models
{
    public struct Sector
    {
        public Sector(IEnumerable<Point> startPoints, IEnumerable<Point> finishPoints)
        {
            var startPointsList = startPoints.ToList();
            if (startPointsList.Count!=2)
                throw new ArgumentException("Wrong amount of start Points");
            StartLine = new Line(startPointsList[0],startPointsList[1]);
        }

        public Line StartLine { get; }
    }
}