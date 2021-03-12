using System;
using FluentAssertions;
using Sanet.SmartSkating.Backend.Azure.Models;
using Sanet.SmartSkating.Dto.Models;
using Xunit;

namespace Sanet.SmartSkating.Backend.Azure.Tests.Models
{
    public class WayPointEntityTests
    {
        [Fact]
        public void CanBeCreatedFromDto()
        {
            var dto = new WayPointDto
            {
                Id = "id",
                SessionId = "sessionId",
                Coordinate = new CoordinateDto
                {
                    Latitude = 1.2,
                    Longitude = 3.2
                },
                Time = DateTime.Now,
                DeviceId = "deviceId"
            };
            var sut = new WayPointEntity(dto);

            sut.PartitionKey.Should().Be(dto.SessionId);
            sut.RowKey.Should().Be(dto.Id);
            sut.Latitude.Should().Be(dto.Coordinate.Latitude);
            sut.Longitude.Should().Be(dto.Coordinate.Longitude);
            sut.Time.Should().Be(dto.Time);
            sut.DeviceId.Should().Be(dto.DeviceId);
        }
        
        [Fact]
        public void CanBeExportedToDto()
        {
            var sut = new WayPointEntity()
            {
                ETag = "tag",
                PartitionKey = "sessionId",
                RowKey = "id",
                Time = DateTime.Now,
                DeviceId = "deviceId",
                Latitude = 456,
                Longitude = 123
            };

            var dto = sut.ToDto();

            dto.Coordinate.Latitude.Should().Be(sut.Latitude);
            dto.Coordinate.Longitude.Should().Be(sut.Longitude);
            dto.Time.Should().Be(sut.Time);
            dto.SessionId.Should().Be(sut.PartitionKey);
            dto.DeviceId.Should().Be(sut.DeviceId);
            dto.Id.Should().Be(sut.RowKey);
        }
    }
}
