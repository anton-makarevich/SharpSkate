using System;
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
        
        private readonly SessionViewModel _sut;

        public SessionViewModelTests()
        {
            _sut = new SessionViewModel(_sessionDto);
        }

        [Fact]
        public void Returns_SessionStart_In_readableFormat()
        {
            const string expectedDate = "20-01-02 8:15";

            _sut.StartDate.Should().Be(expectedDate);
        }
    }
}