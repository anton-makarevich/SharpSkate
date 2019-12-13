using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Sanet.SmartSkating.Utils;

namespace Sanet.SmartSkating.Models.Geometry
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

            Corners = startPointsList;
            if ((startPointsList[1], finishPointsList[0]).GetDistance()
                < (startPointsList[1], finishPointsList[1]).GetDistance())
            {
                Corners.Add(finishPointsList[0]);
                Corners.Add(finishPointsList[1]);
            }
            else
            {
                Corners.Add(finishPointsList[1]);
                Corners.Add(finishPointsList[0]);
            }

            var intersection = (
                new Line(Corners[0], Corners[2]),
                new Line(Corners[1], Corners[3]))
                .GetIntersection();
            
            if (intersection.HasValue)
                Center = intersection.Value;
            else
                throw new NoNullAllowedException("No center point for sector");
        }

        public Line StartLine { get; }
        public Line FinishLine { get; }
        public List<Point> Corners { get; }
        public Point Center { get; }

        public bool Contains(Point point)
        {
            return point.IsInPolygon(Corners.ToArray());
        }
    }   
}