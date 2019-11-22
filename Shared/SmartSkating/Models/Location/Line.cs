namespace Sanet.SmartSkating.Models.Location
{
    public struct Line
    {
        public Line(Point startPoint, Point endPoint)
        {
            Slope = (endPoint.Y-startPoint.Y)/(endPoint.X-startPoint.X);
            Intercept = (startPoint.IsZero)
                ? 0
                : startPoint.Y - Slope * startPoint.X;
        }

        public Line(Point secondPoint) : this(new Point(), secondPoint)
        {
        }

        public double Slope { get; }
        public double Intercept { get;}
    }
}