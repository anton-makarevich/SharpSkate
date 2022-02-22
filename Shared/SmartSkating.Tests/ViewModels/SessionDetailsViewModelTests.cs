using System;
using System.Collections.Generic;
using Acr.UserDialogs;
using FluentAssertions;
using NSubstitute;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Dto.Services;
using Sanet.SmartSkating.Models.Location;
using Sanet.SmartSkating.Models.Training;
using Sanet.SmartSkating.Services.Tracking;
using Sanet.SmartSkating.ViewModels;
using Xunit;

namespace Sanet.SmartSkating.Tests.ViewModels
{
    public class SessionDetailsViewModelTests
    {
        private readonly SessionDetailsViewModel _sut;
        private readonly ISessionManager _sessionManager = Substitute.For<ISessionManager>();
        private readonly IDateProvider _dateProvider = Substitute.For<IDateProvider>();
        private readonly IUserDialogs _userDialogs = Substitute.For<IUserDialogs>();

        public SessionDetailsViewModelTests()
        {
            _sut = new SessionDetailsViewModel(_sessionManager, _dateProvider, _userDialogs);
        }

        [Fact]
        public void FinalSessionTime_Is_Difference_Between_Start_Time_And_Time_Of_Last_WayPoint()
        {
            var session = CreateSessionMock();
            var startTime = new DateTime(2020, 12, 12, 8, 20, 20);
            var lastTime = new DateTime(2020, 12, 12, 8, 40, 40);
            session.StartTime.Returns(startTime);
            session.WayPoints.Returns(new List<WayPoint>()
            {
                new WayPoint(new Coordinate(new CoordinateDto()), lastTime)
            });
            _sessionManager.IsRemote.Returns(true);
            _sessionManager.IsRunning.Returns(false);

            _sut.UpdateUi();

            _sut.FinalSessionTime.Should().Be("0:20:20");
        }

        [Fact]
        public void Subscribes_To_SessionUpdatedEvent_On_Page_Load()
        {
            _sut.AttachHandlers();

            _sessionManager.Received().SessionUpdated += _sut.OnSessionUpdate;
        }

        [Fact]
        public void Unsubscribes_From_SessionUpdatedEvent_On_Page_Unload()
        {
            _sut.DetachHandlers();

            _sessionManager.Received().SessionUpdated -= _sut.OnSessionUpdate;
        }

        [Fact]
        public void ForceUiUpdate_IsTrue_When_Session_Is_Remote_Not_Running_And_Ui_IsNot_Updated()
        {
            _sessionManager.IsRunning.Returns(false);
            _sessionManager.IsRemote.Returns(true);

            _sut.ForceUiUpdate.Should().BeTrue();
        }

        [Fact]
        public void UpdateUi_Updates_Chart_When_Session_Is_Running()
        {
            _sessionManager.IsRunning.Returns(true);
            var session = CreateSessionMock();
            session.LapsCount.Returns(1);
            session.Laps.Returns(new List<Lap>
            {
                new Lap
                {
                    Number = 1,
                    Time = new TimeSpan(123456798)
                }
            });

            _sut.UpdateUi();

            _sut.LapsData.Count.Should().Be(session.Laps.Count);
            _sut.LapsData[0].Y.Should().Be(session.Laps[0].Time.Ticks);
        }

        [Fact]
        public void UpdateUi_Updates_Chart_When_ForceUpdate()
        {
            _sessionManager.IsRunning.Returns(false);
            _sessionManager.IsRemote.Returns(true);

            var session = CreateSessionMock();
            session.LapsCount.Returns(1);
            session.Laps.Returns(new List<Lap>
            {
                new Lap
                {
                    Number = 1,
                    Time = new TimeSpan(123456798)
                }
            });

            _sut.UpdateUi();

            _sut.LapsData.Count.Should().Be(session.Laps.Count);
            _sut.LapsData[0].Y.Should().Be(session.Laps[0].Time.Ticks);
        }

        private ISession CreateSessionMock()
        {
            var session = Substitute.For<ISession>();
            _sessionManager.CurrentSession.Returns(session);
            return session;
        }
    }
}
