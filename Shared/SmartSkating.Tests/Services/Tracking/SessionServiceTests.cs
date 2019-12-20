using Sanet.SmartSkating.Models.Geometry;
using Sanet.SmartSkating.Services.Tracking;
using Sanet.SmartSkating.Tests.Models.Geometry;
using Xunit;

namespace Sanet.SmartSkating.Tests.Services.Tracking
{
    public class SessionServiceTests
    {
        [Fact]
        public void ReturnsSessionForRink()
        {
            var rink = new Rink(RinkTests.EindhovenStart,RinkTests.EindhovenFinish);
            var sut = new SessionService();

            var session = sut.CreateSessionForRink(rink);
            
            Assert.Equal(rink,session.Rink);
        }
    }
}