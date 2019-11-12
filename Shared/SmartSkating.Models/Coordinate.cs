namespace Sanet.SmartSkating.Models
{
    public struct Coordinate
    {
        public Coordinate(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public double Latitude { get;  }
        public double Longitude { get;  }
    }
}