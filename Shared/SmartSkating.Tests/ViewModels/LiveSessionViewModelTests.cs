 using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Acr.UserDialogs;
using FluentAssertions;
using NSubstitute;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Dto.Services;
using Sanet.SmartSkating.Models.EventArgs;
using Sanet.SmartSkating.Models.Location;
using Sanet.SmartSkating.Models.Training;
using Sanet.SmartSkating.Services.Narration;
using Sanet.SmartSkating.Services.Tracking;
using Sanet.SmartSkating.ViewModels;
using Xunit;

namespace Sanet.SmartSkating.Tests.ViewModels
{
    public class LiveSessionViewModelTests
    {
        private readonly LiveSessionViewModel _sut;
        private readonly ISessionManager _sessionManager = Substitute.For<ISessionManager>();
        private readonly IDateProvider _dateProvider = Substitute.For<IDateProvider>();
        private readonly IUserDialogs _userDialogs = Substitute.For<IUserDialogs>();
        private readonly INarratorService _narratorService = Substitute.For<INarratorService>();

        private readonly Coordinate _locationStub = new Coordinate(23, 45);
        private readonly DateTime _testTime = new DateTime(2020, 02, 20, 20, 20, 20);

        public LiveSessionViewModelTests()
        {
            _dateProvider.Now().Returns(_testTime);
            _sut = new LiveSessionViewModel(_sessionManager, _dateProvider, _userDialogs, _narratorService);
        }

        [Fact]
        public void Changes_State_To_IsActive_On_View_Appear()
        {
            _sut.AttachHandlers();

            _sut.IsActive.Should().BeTrue();
        }

        [Fact]
        public void Changes_State_To_Inactive_On_View_Disappear()
        {
            _sut.AttachHandlers();
            _sut.DetachHandlers();
            _sut.IsActive.Should().BeFalse();
        }

        [Fact]
        public void InitialTotalTimeIsZero()
        {
            var expectedTime = new TimeSpan().ToString(LiveSessionViewModel.TotalTimeFormat);
            _sut.TotalTime.Should().Be(expectedTime);
        }

        [Fact]
        public void InitialCurrentSectorIsEqualToEmptyValue()
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
        public void Starts_Session_Checks_Session_State()
        {
            _sessionManager.IsRunning.Returns(true);
            _sut.StartCommand.Execute(null);

            var unused = _sessionManager.Received().IsRunning;
        }

        [Fact]
        public void Starts_Session_Starts_Updating_Session_Time()
        {
            CreateSessionMock();
            _sessionManager.IsRunning.Returns(true);
            _sut.AttachHandlers();

            _sut.StartCommand.Execute(null);

            _dateProvider.Received().Now();
        }

        [Fact]
        public async Task Stops_Session_Button_Ask_Confirmation()
        {
            _sut.StopCommand.Execute(null);
            
            await _userDialogs.Received(1).ConfirmAsync("Do you want to stop session");
        }
        
        [Fact]
        public void Stops_Session_Button_Calls_StopSession_If_Confirmed()
        {
            _userDialogs.ConfirmAsync("Do you want to stop session").Returns(Task.FromResult(true));
            
            _sut.StopCommand.Execute(null);
            
            _sessionManager.Received(1).StopSession();
        }
        
        [Fact]
        public void Stops_Session_Button_DoesNot_Call_StopSession_If_Not_Confirmed()
        {
            _userDialogs.ConfirmAsync("Do you want to stop session").Returns(Task.FromResult(false));
            
            _sut.StopCommand.Execute(null);
            
            _sessionManager.DidNotReceive().StopSession();
        }
        
        [Fact]
        public void Stops_Session_Updates_CanStart_Property()
        {
            _userDialogs.ConfirmAsync("Do you want to stop session").Returns(Task.FromResult(true));
            
            var canStartUpdated = false;
            _sut.PropertyChanged += (sender, args) =>
            {
                canStartUpdated = args.PropertyName == nameof(_sut.CanStart);
            }; 
            
            _sut.StopCommand.Execute(null);

            canStartUpdated.Should().BeTrue();
        }

        [Fact]
        public void InfoLabel_Gets_Updated_When_Session_Is_Running()
        {
            _sessionManager.IsRunning.Returns(true);
            var session = CreateSessionMock();
            session.LastCoordinate.Returns(_locationStub);

            _sut.UpdateUi();

            _sut.InfoLabel.Should().Be(_locationStub.ToString());
        }

        [Fact]
        public void Shows_LastLap_Time_When_Session_Is_Running()
        {
            _sessionManager.IsRunning.Returns(true);
            var session = CreateSessionMock();
            session.LastLapTime.Returns(new TimeSpan(0, 0, 40));
            session.LapsCount.Returns(1);

            _sut.UpdateUi();

            Assert.Equal("00:40",_sut.LastLapTime);
        }

        [Fact]
        public void Shows_Placeholder_For_LastLap_Time_If_No_Laps_Done_And_Session_Is_Running()
        {
            _sessionManager.IsRunning.Returns(true);
            var session = CreateSessionMock();
            session.LapsCount.Returns(0);

            _sut.UpdateUi();

            Assert.Equal(LiveSessionViewModel.NoValue,_sut.LastLapTime);
        }
        
        [Fact]
        public void Shows_Placeholder_For_LastLap_When_Session_HasNot_Started()
        {
            _sut.AttachHandlers();

            Assert.Equal(LiveSessionViewModel.NoValue,_sut.LastLapTime);
        }

        [Fact]
        public void Shows_Best_Lap_Time_When_Session_Is_Running()
        {
            _sessionManager.IsRunning.Returns(true);
            var session = CreateSessionMock();
            session.BestLapTime.Returns(new TimeSpan(0, 0, 40));
            session.LapsCount.Returns(1);

            _sut.UpdateUi();

            Assert.Equal("00:40",_sut.BestLapTime);
        }

        [Fact]
        public void ShowsPlaceholderForBestLapTime_If_Session_Is_Running_And_NoLapsDone()
        {
            _sessionManager.IsRunning.Returns(true);
            var session = CreateSessionMock();
            session.LapsCount.Returns(0);

            _sut.UpdateUi();

            Assert.Equal(LiveSessionViewModel.NoValue,_sut.BestLapTime);
        }

        [Fact]
        public void ShowsPlaceholderForBestLapTime_If_Session_HasNot_Started()
        {
            _sut.AttachHandlers();

            Assert.Equal(LiveSessionViewModel.NoValue,_sut.BestLapTime);
        }

        [Fact]
        public void ShowsAmountOfLaps_When_Session_Is_Running()
        {
            _sessionManager.IsRunning.Returns(true);
            var session = CreateSessionMock();
            session.LapsCount.Returns(1);

            _sut.UpdateUi();

            Assert.Equal("1",_sut.Laps);
        }

        [Fact]
        public void ShowsZeroForAmountOfLaps_If_Session_Is_Running_And_NoLapsDone()
        {
            _sessionManager.IsRunning.Returns(true);
            var session = CreateSessionMock();
            session.LapsCount.Returns(0);

            _sut.UpdateUi();

            Assert.Equal("0",_sut.Laps);
        }

        [Fact]
        public void ShowsLastSectorTime_If_Session_Is_Running()
        {
            _sessionManager.IsRunning.Returns(true);
            CreateSessionMockWithOneSector();

            _sut.UpdateUi();

            Assert.Equal("00:10",_sut.LastSectorTime);
        }

        [Fact]
        public void ShowsPlaceholderForLastSectorTimeIfNoSectorsDone_And_Session_Is_Running()
        {
            _sessionManager.IsRunning.Returns(true);
            var session = CreateSessionMock();
            session.Sectors.Returns(new List<Section>());

            _sut.UpdateUi();

            Assert.Equal(LiveSessionViewModel.NoValue,_sut.LastSectorTime);
        }
        
        [Fact]
        public void ShowsPlaceholderForLastSectorTimeIf_Session_Has_Not_Started()
        {
            _sut.AttachHandlers();

            Assert.Equal(LiveSessionViewModel.NoValue,_sut.LastSectorTime);
        }

        [Fact]
        public void ShowsBestSectorTime_If_Session_Is_Running()
        {
            _sessionManager.IsRunning.Returns(true);
            CreateSessionMockWithOneSector();

            _sut.UpdateUi();

            Assert.Equal("00:10",_sut.BestSectorTime);
        }

        [Fact]
        public void ShowsPlaceholderForBestSectorTimeIfNoSectorsDone_And_Session_Is_Running()
        {
            _sessionManager.IsRunning.Returns(true);
            var session = CreateSessionMock();
            session.Sectors.Returns(new List<Section>());

            _sut.UpdateUi();

            Assert.Equal(LiveSessionViewModel.NoValue,_sut.BestSectorTime);
        }
        
        [Fact]
        public void ShowsPlaceholderForBestSectorTime_When_Session_HasNot_Started()
        {
            _sut.AttachHandlers();

            Assert.Equal(LiveSessionViewModel.NoValue,_sut.BestSectorTime);
        }

        [Fact]
        public void DisplaysTotalDistance_If_Session_Is_Running()
        {
            _sessionManager.IsRunning.Returns(true);
            CreateSessionMockWithOneSector();

            _sut.UpdateUi();

            Assert.Equal("0.1Km",_sut.Distance);
        }
        
        [Fact]
        public void IsRunning_Is_True_When_Session_Has_Started()
        {
            _sessionManager.IsRunning.Returns(true);

            _sut.UpdateUi();

            _sut.IsRunning.Should().BeTrue();
        }

        [Fact]
        public void TotalTime_Is_Zero_When_Session_Is_Not_Running()
        {
            _sessionManager.IsRunning.Returns(false);

            _sut.UpdateUi();

            _sut.TotalTime.Should().Be("0:00:00");
        }

        [Fact]
        public void TotalTime_Is_Zero_Initially()
        {
            _sut.TotalTime.Should().Be("0:00:00");
        }

        [Fact]
        public void TotalTime_Is_Positive_When_Session_Is_Running()
        {
            _sessionManager.IsRunning.Returns(true);
            var session = CreateSessionMock();
            session.StartTime.Returns(_testTime.AddMinutes(-1));

            _sut.UpdateUi();

            _sut.TotalTime.Should().Be("0:01:00");
        }
        
        [Fact]
        public void TotalTime_Is_Not_Being_Updated_When_Session_Is_Stopped()
        {
            _sessionManager.IsRunning.Returns(true);
            var session = CreateSessionMock();
            session.StartTime.Returns(_testTime.AddMinutes(-1));

            _sut.UpdateUi();
            
            _sessionManager.IsRunning.Returns(false);
            session.StartTime.Returns(_testTime.AddMinutes(-4));

            _sut.UpdateUi();

            _sut.TotalTime.Should().Be("0:01:00");
        }
        
        [Fact]
        public void Shows_CurrentSector_When_Session_Is_Running()
        {
            var startWaypoint = new WayPoint(
                _locationStub,
                _locationStub,
                _testTime,
                WayPointTypes.FirstSector);
            
            _sessionManager.IsRunning.Returns(true);
            var session = CreateSessionMock();
            session.WayPoints.Returns(new List<WayPoint> {startWaypoint});

            _sut.UpdateUi();

            _sut.CurrentSector.Should().Be("Currently in 1st sector");
        }

        [Fact]
        public void Start_Updating_OnPageLoad_If_Session_IsRunning()
        {
            _sessionManager.IsRunning.Returns(true);
            CreateSessionMock();
            
            _sut.AttachHandlers();

            var unused = _sessionManager.Received().IsRunning;
        }

        [Fact]
        public void Checks_Session_Type_On_Page_Load()
        {
            _sut.AttachHandlers();
            
            _sessionManager.Received(1).CheckSession();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CanStart_Returns_Manager_Value(bool canStart)
        {
            _sessionManager.CanStart.Returns(canStart);

            _sut.CanStart.Should().Be(canStart);
        }

        [Fact]
        public void StartButton_IsVisible_When_Session_CanStart_And_Local()
        {
            _sessionManager.CanStart.Returns(true);
            _sessionManager.IsRemote.Returns(false);

            _sut.IsStartVisible.Should().BeTrue();
        }
        
        [Fact]
        public void StartButton_IsInvisible_When_Session_CanStart_But_Remote()
        {
            _sessionManager.CanStart.Returns(true);
            _sessionManager.IsRemote.Returns(true);

            _sut.IsStartVisible.Should().BeFalse();
        }
        
        [Fact]
        public void StartButton_IsInvisible_When_Session_CannotStart_And_Local()
        {
            _sessionManager.CanStart.Returns(false);
            _sessionManager.IsRemote.Returns(false);

            _sut.IsStartVisible.Should().BeFalse();
        }
        
        [Fact]
        public void StartButton_IsVisible_When_Session_IsRunning_And_Local()
        {
            _sessionManager.IsRunning.Returns(true);
            _sessionManager.IsRemote.Returns(false);

            _sut.IsStopVisible.Should().BeTrue();
        }
        
        [Fact]
        public void StartButton_IsInvisible_When_Session_IsNotRunning()
        {
            _sessionManager.IsRunning.Returns(false);
            _sessionManager.IsRemote.Returns(false);

            _sut.IsStopVisible.Should().BeFalse();
        }
        
        [Fact]
        public void StartButton_IsInvisible_When_Session_IsRemote()
        {
            _sessionManager.IsRunning.Returns(true);
            _sessionManager.IsRemote.Returns(true);

            _sut.IsStopVisible.Should().BeFalse();
        }

        [Fact]
        public void ForceUiUpdate_Is_Always_False()
        {
            _sut.ForceUiUpdate.Should().BeFalse();
        }
        
        [Fact]
        public void LapPassed_WhenIsBestIsFalse_CallsSpeakTextWithCorrectParameters()
        {
            // Arrange
            var lap = new Lap { Number = 1, Time = new TimeSpan(0, 1, 30) };
            var session = CreateSessionMock();
            _sut.StartCommand.Execute(null);
            
            // Act
            session.LapPassed += Raise.EventWith(null, new LapEventArgs(lap, false));

            // Assert
            _narratorService.Received(1).SpeakText("Lap number 1. 1 minutes, 30 seconds");
        }
        
        [Fact]
        public void LapPassed_WhenIsBestIsTrue_CallsSpeakTextWithCorrectParameters()
        {
            // Arrange
            var lap = new Lap { Number = 1, Time = new TimeSpan(0, 1, 30) };
            var session = CreateSessionMock();
            _sut.StartCommand.Execute(null);
            
            // Act
            session.LapPassed += Raise.EventWith(null, new LapEventArgs(lap, true));

            // Assert
            _narratorService.Received(1).SpeakText("Lap number 1. 1 minutes, 30 seconds. Best lap!");
        }

        private void CreateSessionMockWithOneSector()
        {
            var session = CreateSessionMock();
            var startTime = _testTime;
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
            session.Sectors.Returns(new List<Section> {section});
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
