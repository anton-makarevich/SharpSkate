using System;
using System.Collections.Generic;
using FluentAssertions;
using Sanet.SmartSkating.Dto.Models;
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

        public SessionViewModelTests()
        {
            _sut = new SessionViewModel(_sessionDto, new List<TrackDto>{_trackDto});
        }

        [Fact]
        public void Returns_SessionStart_In_ReadableFormat()
        {
            const string expectedDate = "20-01-02 8:15";

            _sut.StartDate.Should().Be(expectedDate);
        }
        
        [Fact]
        public void Returns_RinkName()
        {
            _sut.RinkName.Should().Be(_trackDto.Name);
        }
        
        [Fact]
        public void Returns_Unknown_For_RinkName_If_Track_NotFound()
        {
            var sut = new SessionViewModel(_sessionDto, new List<TrackDto>());
            
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
            var sut = new SessionViewModel(_sessionDto, new List<TrackDto>());
            
            sut.Status.Should().Be("In progress");
        }

        [Fact]
        public void Returns_Raw_Session_Model()
        {
            _sut.Session.Should().Be(_sessionDto);
        }
    }
}