using System.Globalization;
using Sanet.SmartSkating.Dto.Models;

namespace Sanet.SmartSkating.Models
{
    public struct Coordinate
    {
        public Coordinate(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public Coordinate(CoordinateDto coordinateDto):this(coordinateDto.Latitude,coordinateDto.Longitude)
        {
        }

        public double Latitude { get;  }
        public double Longitude { get;  }

        public override string ToString()
        {
            return $"{Latitude.ToString(CultureInfo.InvariantCulture)};{Longitude.ToString(CultureInfo.InvariantCulture)}";
        }
    }
}