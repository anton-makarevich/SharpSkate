using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Services.Storage;
using Xunit;

namespace Sanet.SmartSkating.Tests.Services.Storage
{
    public class JsonStorageServiceIntegrationTest
    {
        [Fact]
        public async Task SavesWayPointAsJsonFileToLocalFileSystemReadsItAndDeletes()
        {
            var sut = new JsonStorageService();
            var wayPointDto = new WayPointDto
            {
                Coordinate = new CoordinateDto
                {
                    Latitude = 34.56,
                    Longitude = 35.54
                },
                Id = "0",
                SessionId = "8",
                Time = DateTime.Now,
                WayPointType = "uu"
            };

            var isSaved = await sut.SaveWayPointAsync(wayPointDto);
            Assert.True(isSaved);

            var loadedWayPoint = (await sut.GetAllWayPointsAsync()).First();
            loadedWayPoint.Should().BeEquivalentTo(wayPointDto);

            var isDeleted = await sut.DeleteWayPointAsync(loadedWayPoint.Id);
            Assert.True(isDeleted);
        }
        
        [Fact]
        public async Task SavesSessionAsJsonFileToLocalFileSystemReadsItAndDeletes()
        {
            var sut = new JsonStorageService();
            var sessionDto = new SessionDto
            {
                Id = "0",
                AccountId = "8",
            };

            var isSaved = await sut.SaveSessionAsync(sessionDto);
            Assert.True(isSaved);

            var loadedSession = (await sut.GetAllSessionsAsync()).First();
            loadedSession.Should().BeEquivalentTo(sessionDto);

            var isDeleted = await sut.DeleteSessionAsync(loadedSession.Id);
            Assert.True(isDeleted);
        }
        
        [Fact]
        public async Task SavesBleScanAsJsonFileToLocalFileSystemReadsItAndDeletes()
        {
            var sut = new JsonStorageService();
            var bleScanDto = new BleScanResultDto()
            {
                Id = "0",
                DeviceAddress = "8", 
                SessionId = "1",
                Rssi = -77,
                Time = DateTime.UtcNow
            };

            var isSaved = await sut.SaveBleScanAsync(bleScanDto);
            Assert.True(isSaved);

            var loadedScan = (await sut.GetAllBleScansAsync()).First();
            loadedScan.Should().BeEquivalentTo(bleScanDto);

            var isDeleted = await sut.DeleteBleScanAsync(loadedScan.Id);
            Assert.True(isDeleted);
        }
    }
}