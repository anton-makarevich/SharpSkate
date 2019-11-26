using System;
using System.Collections.Generic;
using Sanet.SmartSkating.Utils;

namespace Sanet.SmartSkating.Models.Location
{
    public struct Line
    {
        public Line(Point beginPoint, Point endPoint):this(
            (endPoint.Y-beginPoint.Y)/(endPoint.X-beginPoint.X),
            beginPoint,
            endPoint)
        {
        }
        
        public Line(Point endPoint) : this(new Point(), endPoint)
        {
        }

        private Line(double slope, Point beginPoint, Point? endPoint = null)
        {
            Begin = beginPoint;
            End = endPoint;
            Slope = slope;
            Intercept = (beginPoint.IsZero)
                ? 0
                : beginPoint.Y - Slope * beginPoint.X;
            Length = (End.HasValue)
                // ReSharper disable once PossibleInvalidOperationException
                ? (End.Value.X - Begin.X, 
                    // ReSharper disable once PossibleInvalidOperationException
                    End.Value.Y - Begin.Y).GetDistance()
                : 0;
        }

        public double Slope { get; }
        public double Intercept { get; }
        public double Length { get; }
        public Point Begin { get; }
        public Point? End { get; }
        
        public Line GetPerpendicularToBegin()
        {
            return new Line(-1/this.Slope, this.Begin);
        }

        public Line? GetPerpendicularToEnd()
        {
            if (End.HasValue)    
                return new Line(-1/this.Slope, this.End.Value);
            return null;
        }

        public IEnumerable<Point> FindPointsFromBegin(double distance)
        {
            return FindPointsFrom(Begin, distance);
        }
        
        public IEnumerable<Point> FindPointsFromEnd(double distance)
        {
            return End == null ? new Point[0] : FindPointsFrom(End.Value, distance);
        }

        private IEnumerable<Point> FindPointsFrom(Point point, double distance)
        {
            var d = GetDeltaX(distance);
            var x1 = point.X + d;
            var x2 = point.X - d;
            var y1 = GetY(x1);
            var y2 = GetY(x2);
            return new[] {new Point(x1, y1), new Point(x2,y2)};
        }

        private double GetDeltaX(double distance)
        {
            return distance / Math.Sqrt(1 + Slope * Slope);
        }

        public double GetY(double x)
        {
            return x * Slope + Intercept;
        }
    }
}