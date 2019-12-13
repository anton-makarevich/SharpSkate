using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Sanet.SmartSkating.Models.Training;
using Sanet.SmartSkating.Utils;

namespace Sanet.SmartSkating.Models.Geometry
{
    public struct Sector
    {
        public Sector(IEnumerable<Point> startPoints, IEnumerable<Point> finishPoints, WayPointTypes type)
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

            Type = type;
        }

        public Line StartLine { get; }
        public Line FinishLine { get; }
        public List<Point> Corners { get; }
        public Point Center { get; }
        public WayPointTypes Type { get; }

        public bool Contains(Point point)
        {
            return point.IsInPolygon(Corners.ToArray());
        }

        public Point? FindIntersection(Line line)
        {
            var pointsInSector = 0;
            foreach (var point in new []{line.Begin,line.End})
            {
                if (Contains(point))
                    pointsInSector++;
            }

            if (pointsInSector != 1)
                return null;
                
            var length = line.Length;

            for (var i = 0; i < Corners.Count - 1; i++)
            {
                var side = new Line(Corners[i],Corners[i+1]);
                var intersection = (line, side).GetIntersection();
                if (intersection.HasValue
                    && (intersection.Value, line.Begin).GetDistance() < length)
                    return intersection;
            }

            return null;
        }
    }   
}