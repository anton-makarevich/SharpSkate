using NSubstitute;
using Sanet.SmartSkating.Models.Geometry;
using Sanet.SmartSkating.Services;
using Sanet.SmartSkating.Services.Tracking;
using Sanet.SmartSkating.Tests.Models.Geometry;
using Xunit;

namespace Sanet.SmartSkating.Tests.Services.Tracking
{
    public class SessionServiceTests
    {
        private readonly ISettingsService _settingsService = Substitute.For<ISettingsService>();

        [Fact]
        public void ReturnsSessionForRink()
        {
            var rink = new Rink(RinkTests.EindhovenStart,RinkTests.EindhovenFinish,"rinkId");
            var sut = new SessionService(_settingsService);

            var session = sut.CreateSessionForRink(rink);

            Assert.Equal(rink,session.Rink);
        }
    }
}
