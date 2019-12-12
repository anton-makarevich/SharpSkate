using System;

namespace Sanet.SmartSkating.Utils
{
    public static class GeoLocationExtensions
    {
        public static double ToLatitudeDistanceInMeters(this double latitudeDifference)
        {
            return latitudeDifference*111111;
        }
        
        public static double ToLongitudeDistanceInMeters(this double longitudeDifference, double longitudeFactor)
        {
            return longitudeDifference * 111321 * longitudeFactor;
        }
        
        public static double ToLatitudeDistanceInDegrees(this double xDifference)
        {
            return xDifference*1/111111;
        }
        
        public static double ToLongitudeDistanceInDegrees(this double yDifference, double longitudeFactor)
        {
            return yDifference /( 111321 * longitudeFactor);
        }

        public static double GetLongitudeFactor(this double longitude)
        {
            return Math.Cos(longitude.ToRadians());
        }
    }
}