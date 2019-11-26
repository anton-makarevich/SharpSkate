using System;
using Sanet.SmartSkating.Models.Location;

namespace Sanet.SmartSkating.Utils
{
    public static class MathExtensions
    {
        public static double ToRadians(this double degrees)
        {
            return degrees* Math.PI/180;
        }

        public static double GetDistance(this (double, double) delta)
        {
#pragma warning disable SA1407
            return Math.Sqrt(delta.Item1*delta.Item1+delta.Item2*delta.Item2);
        }
        
        public static double GetDistance(this (Point, Point) points)
        {
            var dx = points.Item2.X - points.Item1.X;
            var dy = points.Item2.Y - points.Item1.Y;
            return (dx,dy).GetDistance();
        }
    }
}