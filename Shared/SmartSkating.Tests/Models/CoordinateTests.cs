using Sanet.SmartSkating.Models;
using Xunit;

namespace Sanet.SmartSkating.Tests.Models
{
    public class CoordinateTests
    {
        [Fact]
        public void ToStringPrintsLatitudeAndLongitude()
        {
            var sut = new Coordinate(34.45, 23.56);
            
            var text = sut.ToString();
            
            Assert.Equal("34.45;23.56",text);
        }
    }
}