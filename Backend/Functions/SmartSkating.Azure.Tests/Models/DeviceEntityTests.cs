using FluentAssertions;
using Sanet.SmartSkating.Backend.Azure.Models;
using Sanet.SmartSkating.Dto.Models;
using Xunit;

namespace Sanet.SmartSkating.Backend.Azure.Tests.Models
{
    public class DeviceEntityTests
    {
        [Fact]
        public void CanBeCreatedFromDto()
        {
            var dto = new DeviceDto
            {
                Id = "id",
                Manufacturer = "apple",
                Model = "iPhone",
                OsName = "iOS",
                OsVersion = "13.1"
            };
            var sut = new DeviceEntity(dto);

            sut.PartitionKey.Should().Be(dto.AccountId);
            sut.RowKey.Should().Be(dto.Id);
            sut.OsInfo.Should().Be($"{dto.OsName}:{dto.OsVersion}");
            sut.DeviceInfo.Should().Be($"{dto.Manufacturer}-{dto.Model}");
        }
    }
}