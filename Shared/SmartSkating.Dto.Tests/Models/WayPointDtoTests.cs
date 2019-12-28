using System;
using Sanet.SmartSkating.Dto.Models;
using Xunit;

namespace SmartSkating.Dto.Tests.Models
{
    public class WayPointDtoTests
    {
        [Fact]
        public void CreatesWayPointDtoFromSessionCoordinate()
        {
            var sessionId = Guid.NewGuid().ToString("N");
            var coordinate = new CoordinateDto {Latitude = 45,Longitude = 34};
            var result = WayPointDto.FromSessionCoordinate(sessionId, coordinate);
            
            Assert.Equal(sessionId, result.SessionId);
            Assert.Equal(coordinate,result.Coordinate);
        }
    }
}