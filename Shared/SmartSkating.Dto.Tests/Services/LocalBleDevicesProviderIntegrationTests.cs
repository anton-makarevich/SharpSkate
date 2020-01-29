using System.Threading.Tasks;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Dto.Services;
using Xunit;

namespace SmartSkating.Dto.Tests.Services
{
    public class LocalBleDevicesProviderIntegrationTests
    {
        [Fact]
        public async Task ReturnsListOfDevicesFromLocalJsonFile()
        {
            var sut = new LocalBleDevicesProvider(new EmbeddedResourceReader());

            var localTracks = await sut.GetBleDevicesAsync();
            
            Assert.NotEmpty(localTracks);
        }
    }
}