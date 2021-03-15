using FluentAssertions;
using Sanet.SmartSkating.Dto.Models;
using Xunit;

namespace SmartSkating.Dto.Tests.Models
{
    public class SyncHubMethodNamesTests
    {
        [Fact]
        public void SyncHubMethodNames_Have_Correct_Values()
        {
            SyncHubMethodNames.AddWaypoint.Should().Be("newWaypoint");
            SyncHubMethodNames.EndSession.Should().Be("endSession");
        }
    }
}