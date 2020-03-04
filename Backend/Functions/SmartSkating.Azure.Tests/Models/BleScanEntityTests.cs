using System;
using FluentAssertions;
using Sanet.SmartSkating.Backend.Azure.Models;
using Sanet.SmartSkating.Dto.Models;
using Xunit;

namespace Sanet.SmartSkating.Backend.Azure.Tests.Models
{
    public class BleScanEntityTests
    {
        [Fact]
        public void CanBeCreatedFromDto()
        {
            var dto = new BleScanResultDto
            {
                Id = "id",
                DeviceAddress = "deviceId",
                Rssi = -65,
                Time = DateTime.Now,
                SessionId = "sessionId"
            };
            var sut = new BleScanEntity(dto);

            sut.PartitionKey.Should().Be(dto.SessionId);
            sut.RowKey.Should().Be(dto.Id);
            sut.Rssi.Should().Be(dto.Rssi);
            sut.Time.Should().Be(dto.Time);
            sut.DeviceId.Should().Be(dto.DeviceAddress);
        }
    }
}