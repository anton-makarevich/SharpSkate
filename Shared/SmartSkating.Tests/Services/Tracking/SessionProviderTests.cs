using FluentAssertions;
using NSubstitute;
using Sanet.SmartSkating.Models.Geometry;
using Sanet.SmartSkating.Services;
using Sanet.SmartSkating.Services.Tracking;
using Sanet.SmartSkating.Tests.Models.Geometry;
using Xunit;

namespace Sanet.SmartSkating.Tests.Services.Tracking
{
    public class SessionProviderTests
    {
        private readonly ISettingsService _settingsService = Substitute.For<ISettingsService>();

        private readonly SessionProvider _sut;

        public SessionProviderTests()
        {
            _sut = new SessionProvider(_settingsService);
        }

        [Fact]
        public void ReturnsSessionForRink()
        {
            var rink = new Rink(RinkTests.EindhovenStart,RinkTests.EindhovenFinish,"rinkId");

            var session = _sut.CreateSessionForRink(rink);

            session.Rink.Should().Be(rink);
        }

        [Fact]
        public void Assigns_CurrentSession_Property_When_New_Session_IsCreated()
        {
            var rink = new Rink(RinkTests.EindhovenStart,RinkTests.EindhovenFinish,"rinkId");

            var session = _sut.CreateSessionForRink(rink);

            _sut.CurrentSession.Should().Be(session);
        }
    }
}
