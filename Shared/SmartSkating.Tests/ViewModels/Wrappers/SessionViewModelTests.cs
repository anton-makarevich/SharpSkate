using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Services.Api;
using Sanet.SmartSkating.ViewModels.Wrappers;
using Xunit;

namespace Sanet.SmartSkating.Tests.ViewModels.Wrappers
{
    public class SessionViewModelTests
    {
        private readonly SessionDto _sessionDto = new SessionDto
        {
            Id = "sessionId",
            AccountId = "accountId",
            IsCompleted = true,
            IsSaved = true,
            RinkId = "rinkId",
            DeviceId = "deviceId",
            StartTime = new DateTime(2020, 01, 02, 8, 15, 00)
        };

        private readonly TrackDto _trackDto = new TrackDto
        {
            Id = "rinkId",
            Name = "Some Rink",
            Start = default,
            Finish = default
        };

        private readonly SessionViewModel _sut;
        private readonly IDataSyncService _dataSyncService;

        public SessionViewModelTests()
        {
            _dataSyncService = Substitute.For<IDataSyncService>();
            _sut = new SessionViewModel(_sessionDto, new List<TrackDto>{_trackDto},_dataSyncService);
        }

        [Fact]
        public void Returns_SessionStart_In_ReadableFormat()
        {
            const string expectedDate = "20-01-02 8:15";

            _sut.StartTime.Should().Be(expectedDate);
        }

        [Fact]
        public void Returns_Session_StartTime_In_Readable_Pm_Format()
        {
            _sessionDto.StartTime = new DateTime(2020, 01, 02, 21, 15, 00);
            var sut = new SessionViewModel(_sessionDto, new List<TrackDto>(),_dataSyncService);
            const string expectedDate = "20-01-02 21:15";

            sut.StartTime.Should().Be(expectedDate);
        }

        [Fact]
        public void Returns_RinkName()
        {
            _sut.RinkName.Should().Be(_trackDto.Name);
        }

        [Fact]
        public void Returns_Unknown_For_RinkName_If_Track_NotFound()
        {
            var sut = new SessionViewModel(_sessionDto, new List<TrackDto>(),_dataSyncService);

            sut.RinkName.Should().Be("Unknown");
        }

        [Fact]
        public void Returns_Completed_For_Status_If_Session_Is_Completed()
        {
            _sut.Status.Should().Be("Completed");
        }

        [Fact]
        public void Returns_InProgress_For_Status_If_Session_Is_NotCompleted()
        {
            _sessionDto.IsCompleted = false;
            var sut = new SessionViewModel(_sessionDto, new List<TrackDto>(),_dataSyncService);

            sut.Status.Should().Be("In progress");
        }

        [Fact]
        public void Returns_Raw_Session_Model()
        {
            _sut.Session.Should().Be(_sessionDto);
        }

        [Fact]
        public async Task CompleteSessionCommand_Calls_SaveAndSyncSession_And_Session_IsCompleted()
        {
            await _sut.CompleteSessionCommand.ExecuteAsync();

            await _dataSyncService.Received(1).SaveAndSyncSessionAsync(_sessionDto);
            _sessionDto.IsCompleted.Should().BeTrue();
        }

        [Fact]
        public async Task CompleteSession_Raises_Event_When_Session_Status_Is_Updated()
        {
            var sessionUpdatedIsCalled = false;

            _sut.SessionUpdated += () =>
            {
                sessionUpdatedIsCalled = true;
            };

            await _sut.CompleteSessionCommand.ExecuteAsync();

            sessionUpdatedIsCalled.Should().BeTrue();
        }
    }
}
