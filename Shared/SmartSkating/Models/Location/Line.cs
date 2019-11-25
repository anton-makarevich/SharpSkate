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
        public double Intercept { get;}
        public double Length { get; }
        public Point Begin { get;}
        public Point? End { get;}
        
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
    }
}