using System;
using System.Linq;
using Sanet.SmartSkating.Models.Location;

namespace Sanet.SmartSkating.Utils
{
    public static class MathExtensions
    {
        private const double RoundTolerance = 0.001;
        
        public static double ToRadians(this double degrees)
        {
            return degrees* Math.PI/180;
        }

        public static double GetDistance(this (double, double) delta)
        {
            return Math.Sqrt(delta.Item1*delta.Item1+delta.Item2*delta.Item2);
        }
        
        public static double GetDistance(this (Point, Point) points)
        {
            var dx = points.Item2.X - points.Item1.X;
            var dy = points.Item2.Y - points.Item1.Y;
            return (dx,dy).GetDistance();
        }
        
        public static bool IsInPolygon(this Point point, Point[] polygon)
        {
            var result = false;
            var a = polygon.Last();
            foreach (var b in polygon)
            {
                if (Math.Abs(b.X - point.X) < RoundTolerance && Math.Abs(b.Y - point.Y) < RoundTolerance)
                    return true;

                if (Math.Abs(b.Y - a.Y) < RoundTolerance && 
                    Math.Abs(point.Y - a.Y) < RoundTolerance && 
                    a.X <= point.X && 
                    point.X <= b.X)
                    return true;

                if (b.Y < point.Y && a.Y >= point.Y || a.Y < point.Y && b.Y >= point.Y)
                {
                    if (b.X + (point.Y - b.Y) / (a.Y - b.Y) * (a.X - b.X) <= point.X)
                        result = !result;
                }
                a = b;
            }
            return result;
        }
        
        public static bool Contains(this Line line, Point point)
        {
            return (double.IsInfinity(line.Slope))
                ? Math.Abs(point.X - line.Begin.X) < RoundTolerance
                : Math.Abs(line.GetY(point.X) - point.Y) < RoundTolerance;
        }
    }
}