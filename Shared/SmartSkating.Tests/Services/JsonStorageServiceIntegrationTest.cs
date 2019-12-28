using System;
using System.Threading.Tasks;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Services.Storage;
using Xunit;

namespace Sanet.SmartSkating.Tests.Services
{
    public class JsonStorageServiceIntegrationTest
    {
        [Fact]
        public async Task SavesJsonFileToLocalFileSystem()
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

            await sut.SaveWayPointAsync(wayPointDto);
        }
    }
}