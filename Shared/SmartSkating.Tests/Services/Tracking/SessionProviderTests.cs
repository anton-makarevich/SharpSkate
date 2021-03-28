using System;
using FluentAssertions;
using NSubstitute;
using Sanet.SmartSkating.Dto.Models;
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
        
        [Fact]
        public void CurrentSession_Id_Equal_To_Given_SessionDto_Id()
        {
            var sessionDto = new SessionDto {Id = "sessionDtoId"};
            var rink = new Rink(RinkTests.EindhovenStart,RinkTests.EindhovenFinish,"rinkId");

            _sut.SetActiveSession(sessionDto,rink);

            (_sut.CurrentSession?.SessionId).Should().Be(sessionDto.Id);
        }
        
        [Fact]
        public void CurrentSession_StartTime_Equal_To_StartTime_Of_SessionDto()
        {
            var startTime = DateTime.Now;
            var sessionDto = new SessionDto {StartTime = startTime};
            var rink = new Rink(RinkTests.EindhovenStart,RinkTests.EindhovenFinish,"rinkId");

            _sut.SetActiveSession(sessionDto,rink);

            (_sut.CurrentSession?.StartTime).Should().Be(startTime);
        }
        
        [Fact]
        public void CurrentSession_IsCompleted_Equal_To_IsCompleted_Of_SessionDto()
        {
            var startTime = DateTime.Now;
            var sessionDto = new SessionDto {IsCompleted = true};
            var rink = new Rink(RinkTests.EindhovenStart,RinkTests.EindhovenFinish,"rinkId");

            _sut.SetActiveSession(sessionDto,rink);

            (_sut.CurrentSession?.IsCompleted).Should().BeTrue();
        }
    }
}
