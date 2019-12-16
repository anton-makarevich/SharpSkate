using System.Threading.Tasks;
using Sanet.SmartSkating.Dto.Services;
using Xunit;

namespace SmartSkating.Dto.Tests.Services
{
    public class LocalTrackProviderIntegrationTests
    {
        [Fact]
        public async Task ReturnsListOfTracksFromLocalJsonFile()
        {
            var sut = new LocalTrackProvider();

            var localTracks = await sut.GetAllTracksAsync();
            
            Assert.NotEmpty(localTracks);
        }
    }
}