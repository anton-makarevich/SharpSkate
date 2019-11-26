using Sanet.SmartSkating.Models.Location;
using Sanet.SmartSkating.Utils;

namespace Sanet.SmartSkating.Models
{
    public class Rink
    {
        public Rink(Coordinate start, Coordinate finish)
        {
            Start = start;
            Finish = finish;

            var longitudeFactor = Start.Longitude.GetLongitudeFactor();
            
            StartLocal = new Point();

            var latitudeToFinish = finish.Latitude - start.Latitude;
            var longitudeToFinish = finish.Longitude - start.Longitude;

            FinishLocal = new Point(latitudeToFinish.ToLatitudeDistanceInMeters(),longitudeToFinish.ToLongitudeDistanceInMeters(longitudeFactor));
        }

        public Coordinate Start { get; }
        public Coordinate Finish { get; }
        public Point StartLocal { get; }
        public Point FinishLocal { get; }
    }
}