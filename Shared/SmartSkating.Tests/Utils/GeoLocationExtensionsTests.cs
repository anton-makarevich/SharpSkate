using Sanet.SmartSkating.Utils;
using Xunit;

namespace Sanet.SmartSkating.Tests.Utils
{
    public class GeoLocationExtensionsTests
    {
        [Fact]
        public void CorrectlyCalculatesLatitudeDelta()
        {
            const double latitude1 = 45;
            const double latitude2 = 45.00001;

            var result = (latitude2 - latitude1).ToLatitudeDistanceInMeters();
            
            Assert.Equal(1.11,result, 2);
        }
        
        [Fact]
        public void CorrectlyCalculatesLongitudeDelta()
        {
            const double longitude1 = 45;
            const double longitude2 = 45.00001;

            const double longitudeFactor = 0.707106781;

            var result = (longitude2 - longitude1).ToLongitudeDistanceInMeters(longitudeFactor);
            
            Assert.Equal(0.79,result, 2);
        }
        
        [Fact]
        public void CorrectlyConvertsLatitudeDeltaBackToDegrees()
        {
            const double x1 = 45;    
            const double x2 = 46.11;

            var result = (x2 - x1).ToLatitudeDistanceInDegrees();
            
            Assert.Equal(0.00001,result, 5);
        }
        
        [Fact]
        public void CorrectlyConvertsLongitudeDeltaBackToDegrees()
        {
            const double y1 = 45;
            const double y2 = 45.79;

            const double longitudeFactor = 0.707106781;

            var result = (y2 - y1).ToLongitudeDistanceInDegrees(longitudeFactor);
            
            Assert.Equal(0.00001,result, 5);
        }

        [Fact]
        public void CalculatesLongitudeFactorFromLongitudeValue()
        {
            const double longitude = 45;

            var result = longitude.GetLongitudeFactor();
            
            Assert.Equal(0.70710678,result,8);
        }
    }
}