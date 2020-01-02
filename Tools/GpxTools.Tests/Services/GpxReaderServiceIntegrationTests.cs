using System.Threading.Tasks;
using GpxTools.Services;
using Xunit;

namespace GpxTools.Tests.Services
{
    public class GpxReaderServiceIntegrationTests
    {
        [Fact]
        public async Task ReadsEmbeddedGpxFile()
        {
            var sut = new GpxReaderService();

            var result = await sut.ReadEmbeddedGpxFileAsync("Grefrath");
            
            Assert.NotEmpty(result.RoutePoints);
        }
    }
}