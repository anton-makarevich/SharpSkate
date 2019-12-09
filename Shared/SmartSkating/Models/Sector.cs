using System;
using System.Collections.Generic;
using System.Linq;
using Sanet.SmartSkating.Models.Location;
using Sanet.SmartSkating.Utils;

namespace Sanet.SmartSkating.Models
{
    public struct Sector
    {
        public Sector(IEnumerable<Point> startPoints, IEnumerable<Point> finishPoints)
        {
            var startPointsList = startPoints.ToList();
            if (startPointsList.Count!=2)
                throw new ArgumentException("Wrong amount of start Points");
            
            var finishPointsList = finishPoints.ToList();
            if (finishPointsList.Count!=2)
                throw new ArgumentException("Wrong amount of finish Points");
            
            StartLine = new Line(startPointsList[0],startPointsList[1]);
            
            FinishLine = new Line(finishPointsList[0],finishPointsList[1]);

            Points = startPointsList;
            if ((startPointsList[1], finishPointsList[0]).GetDistance()
                < (startPointsList[1], finishPointsList[1]).GetDistance())
            {
                Points.Add(finishPointsList[0]);
                Points.Add(finishPointsList[1]);
            }
            else
            {
                Points.Add(finishPointsList[1]);
                Points.Add(finishPointsList[0]);
            }
        }

        public Line StartLine { get; }
        public Line FinishLine { get; }
        public List<Point> Points { get; } 

        public bool Contains(Point point)
        {
            return point.IsInPolygon(Points.ToArray());
        }
    }   
}