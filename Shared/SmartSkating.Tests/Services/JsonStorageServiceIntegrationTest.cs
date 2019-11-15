using System.Threading.Tasks;
using Sanet.SmartSkating.Models;
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
            var coordinateStub = new Coordinate(34.5454, 45.23343);

            await sut.SaveCoordinateAsync(coordinateStub);
        }
    }
}