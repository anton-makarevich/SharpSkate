using FluentAssertions;
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
        private readonly Rink _rink = new Rink(RinkTests.EindhovenStart,RinkTests.EindhovenFinish,"rinkId");

        private readonly SessionService _sut;

        public SessionServiceTests()
        {
            _sut = new SessionService(_settingsService);
        }

        [Fact]
        public void ReturnsSessionForRink()
        {
            var session = _sut.CreateSessionForRink(_rink);

            Assert.Equal(_rink,session.Rink);
        }

        [Fact]
        public void CreateSession_Assigns_Value_To_CurrentSession()
        {

            var session = _sut.CreateSessionForRink(_rink);

            _sut.CurrentSession.Should().Be(session);
        }
    }
}
