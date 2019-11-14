namespace Sanet.SmartSkating.Models.EventArgs
{
    public class CoordinateEventArgs:System.EventArgs
    {
        public Coordinate Coordinate { get; }

        public CoordinateEventArgs(Coordinate coordinate)
        {
            Coordinate = coordinate;
        }
    }
}