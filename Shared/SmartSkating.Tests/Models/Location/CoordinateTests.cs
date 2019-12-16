using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Models;
using Xunit;

namespace Sanet.SmartSkating.Tests.Models.Location
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

        [Fact]
        public void CreatesCoordinateFromDto()
        {
            var coordinateDto = new CoordinateDto {Latitude = 34.56,Longitude = 23.45};
            
            var coordinate = new Coordinate(coordinateDto);
            
            Assert.Equal(coordinateDto.Latitude,coordinate.Latitude,5);
            Assert.Equal(coordinateDto.Longitude,coordinate.Longitude,5);
        }
    }
    
}