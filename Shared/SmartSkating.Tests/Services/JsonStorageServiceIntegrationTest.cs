using System;
using System.Linq;
using System.Threading.Tasks;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Services.Storage;
using Xunit;

namespace Sanet.SmartSkating.Tests.Services
{
    public class JsonStorageServiceIntegrationTest
    {
        [Fact]
        public async Task SavesWayPointAsJsonFileToLocalFileSystemReadsItAndDeletes()
        {
            var sut = new JsonStorageService();
            var wayPointDto = new WayPointDto()
            {
                Coordinate = new CoordinateDto()
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
            Assert.Equal(wayPointDto, loadedWayPoint);

            var isDeleted = await sut.DeleteWayPointAsync(loadedWayPoint.Id);
            Assert.True(isDeleted);
        }
        
        
    }
}