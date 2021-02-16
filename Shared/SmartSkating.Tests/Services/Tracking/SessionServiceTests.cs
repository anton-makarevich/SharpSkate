using FluentAssertions;
using NSubstitute;
using Sanet.SmartSkating.Dto.Services;
using Sanet.SmartSkating.Models.Geometry;
using Sanet.SmartSkating.Services;
using Sanet.SmartSkating.Services.Account;
using Sanet.SmartSkating.Services.Api;
using Sanet.SmartSkating.Services.Location;
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
        private readonly ILocationService _locationService = Substitute.For<ILocationService>();
        private readonly IDataService _dataService = Substitute.For<IDataService>();
        private readonly ITrackService _trackService = Substitute.For<ITrackService>();
        private readonly IAccountService _accountService = Substitute.For<IAccountService>();
        private readonly IDataSyncService _dataSyncService = Substitute.For<IDataSyncService>();
        private readonly IBleLocationService _bleLocationService = Substitute.For<IBleLocationService>();

        public SessionServiceTests()
        {
            _sut = new SessionService(_locationService,
                _dataService,
                _trackService,
                _accountService,
                _dataSyncService,
                _bleLocationService,
                _settingsService);
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

        [Fact]
        public void StartsLocationServiceWhenStartButtonPressed()
        {
            _sut.StartSession();

            _locationService.Received().StartFetchLocation();
        }
    }
}
