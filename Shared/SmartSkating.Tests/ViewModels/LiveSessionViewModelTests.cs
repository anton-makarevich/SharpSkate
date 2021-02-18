using System;
using System.Collections.Generic;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Models.Location;
using Sanet.SmartSkating.Models.Training;
using Sanet.SmartSkating.Services.Tracking;
using Sanet.SmartSkating.ViewModels;
using Xunit;

namespace Sanet.SmartSkating.Tests.ViewModels
{
    public class LiveSessionViewModelTests
    {
        private readonly LiveSessionViewModel _sut;
        private readonly ISessionManager _sessionManager = Substitute.For<ISessionManager>();
        private readonly Coordinate _locationStub = new Coordinate(23, 45);

        public LiveSessionViewModelTests()
        {
            _sut = new LiveSessionViewModel(_sessionManager);
        }

        [Fact]
        public void Changes_State_To_IsActive_On_View_Appear()
        {
            _sut.AttachHandlers();

            _sut.IsActive.Should().Be(true);
        }

        [Fact]
        public void Changes_State_To_Inactive_On_View_Disappear()
        {
            _sut.AttachHandlers();
            _sut.DetachHandlers();
            _sut.IsActive.Should().Be(false);
        }

        [Fact]
        public void InitialTotalTimeIsZero()
        {
            var expectedTime = new TimeSpan().ToString(LiveSessionViewModel.TotalTimeFormat);
            _sut.TotalTime.Should().Be(expectedTime);
        }

        [Fact]
        public void InitialCurrentSessionIsEqualToEmptyValue()
        {
            _sut.CurrentSector.Should().Be(LiveSessionViewModel.NoValue);
        }

        [Fact]
        public void Starts_Session_WhenStartButtonPressed()
        {
            _sut.StartCommand.Execute(null);

            _sessionManager.Received().StartSession();
        }

        [Fact]
        public void LastCoordinateChange_Updates_InfoLabel()
        {
            var session = CreateSessionMock();
            session.LastCoordinate.Returns(_locationStub);

            _sut.UpdateUi();

            _sut.InfoLabel.Should().Be(_locationStub.ToString());
        }

        [Fact]
        public void ShowsLastLapTime()
        {
            var session = CreateSessionMock();
            session.LastLapTime.Returns(new TimeSpan(0, 0, 40));
            session.LapsCount.Returns(1);

            _sut.UpdateUi();

            Assert.Equal("0:00:40",_sut.LastLapTime);
        }

        [Fact]
        public void ShowsPlaceholderForLastLapTimeIfNoLapsDone()
        {
            var session = CreateSessionMock();
            session.LapsCount.Returns(0);

            _sut.UpdateUi();

            Assert.Equal(LiveSessionViewModel.NoValue,_sut.LastLapTime);
        }

        [Fact]
        public void ShowsBestLapTime()
        {
            var session = CreateSessionMock();
            session.BestLapTime.Returns(new TimeSpan(0, 0, 40));
            session.LapsCount.Returns(1);

            _sut.UpdateUi();

            Assert.Equal("0:00:40",_sut.BestLapTime);
        }

        [Fact]
        public void ShowsPlaceholderForBestLapTimeIfNoLapsDone()
        {
            var session = CreateSessionMock();
            session.LapsCount.Returns(0);

            _sut.UpdateUi();

            Assert.Equal(LiveSessionViewModel.NoValue,_sut.BestLapTime);
        }

        [Fact]
        public void ShowsAmountOfLaps()
        {
            var session = CreateSessionMock();
            session.LapsCount.Returns(1);

            _sut.UpdateUi();

            Assert.Equal("1",_sut.Laps);
        }

        [Fact]
        public void ShowsZeroForAmountOfLapsIfNoLapsDone()
        {
            var session = CreateSessionMock();
            session.LapsCount.Returns(0);

            _sut.UpdateUi();

            Assert.Equal("0",_sut.Laps);
        }

        [Fact]
        public void ShowsLastSectorTime()
        {
            CreateSessionMockWithOneSector();

            _sut.UpdateUi();

            Assert.Equal("00:10",_sut.LastSectorTime);
        }

        [Fact]
        public void ShowsPlaceholderForLastSectorTimeIfNoSectorsDone()
        {
            var session = CreateSessionMock();
            session.Sectors.Returns(new List<Section>());

            _sut.UpdateUi();

            Assert.Equal(LiveSessionViewModel.NoValue,_sut.LastSectorTime);
        }

        [Fact]
        public void ShowsBestSectorTime()
        {
            CreateSessionMockWithOneSector();

            _sut.UpdateUi();

            Assert.Equal("00:10",_sut.BestSectorTime);
        }

        [Fact]
        public void ShowsPlaceholderForBestSectorTimeIfNoSectorsDone()
        {
            var session = CreateSessionMock();
            session.Sectors.Returns(new List<Section>());

            _sut.UpdateUi();

            Assert.Equal(LiveSessionViewModel.NoValue,_sut.BestSectorTime);
        }

        [Fact]
        public void DisplaysTotalDistance()
        {
            CreateSessionMockWithOneSector();

            _sut.UpdateUi();

            Assert.Equal("0.1Km",_sut.Distance);
        }

        [Fact]
        public void StartingSessionUpdatesItsStartTime()
        {
            var session = CreateSessionMock();
            var startTime = DateTime.Now;
            var endTime = startTime.AddSeconds(10);
            var section = new Section(
                new WayPoint(_locationStub,_locationStub,startTime, WayPointTypes.Start),
                new WayPoint(_locationStub,_locationStub,endTime, WayPointTypes.Finish)
            );
            session.Sectors.Returns(new List<Section>(){section});

            _sut.StartCommand.Execute(null);

            session.Received().SetStartTime(Arg.Any<DateTime>());
        }

        [Fact]
        public void CanStart_When_Session_Exists()
        {
            CreateSessionMock();

            _sut.CanStart.Should().BeTrue();
        }

        [Fact]
        public void CanNotStart_When_Session_DoesNot_Exist()
        {
            _sessionManager.CurrentSession.ReturnsNull();
            _sut.CanStart.Should().BeFalse();
        }

        private void CreateSessionMockWithOneSector()
        {
            var session = CreateSessionMock();
            var startTime = DateTime.Now;
            var endTime = startTime.AddSeconds(10);
            var section = new Section(
                new WayPoint(
                    _locationStub,
                    _locationStub,
                    startTime,
                    WayPointTypes.Start),
                new WayPoint(
                    _locationStub,
                    _locationStub,
                    endTime,
                    WayPointTypes.Finish)
            );
            session.Sectors.Returns(new List<Section>() {section});
            session.BestSector.Returns(section);
        }

        private ISession CreateSessionMock()
        {
            var session = Substitute.For<ISession>();
            _sessionManager.CurrentSession.Returns(session);
            return session;
        }
    }
}
