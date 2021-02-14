using FluentAssertions;
using Sanet.SmartSkating.Dto.Models;
using Xunit;

namespace SmartSkating.Dto.Tests.Models
{
    public class WayPointDtoTests
    {
        [Fact]
        public void CreatesWayPointDtoFromSessionCoordinate()
        {
            const string sessionId = "sessionId";
            const string deviceId = "deviceId";
            var coordinate = new CoordinateDto {Latitude = 45,Longitude = 34};
            var result = WayPointDto.FromSessionCoordinate(sessionId, deviceId, coordinate);

            result.SessionId.Should().Be(sessionId);
            result.DeviceId.Should().Be(deviceId);
            result.Coordinate.Should().Be(coordinate);
        }
    }
}
