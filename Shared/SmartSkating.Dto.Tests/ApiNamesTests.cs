using FluentAssertions;
using Sanet.SmartSkating.Dto;
using Xunit;

namespace SmartSkating.Dto.Tests
{
    public class ApiNamesTests
    {
        private readonly string _wayPointsApiName = "waypoints";
        private readonly string _sessionsApiName = "sessions";
        
        [Fact]
        public void WayPointsApiResourceHasCorrectNames()
        {
            ApiNames.WayPointsResource.Path.Should().Be($"/{_wayPointsApiName}");
            ApiNames.WayPointsResource.Route.Should().Be(_wayPointsApiName);
        }
        
        [Fact]
        public void SessionsApiResourceHasCorrectName()
        {
            ApiNames.SessionsResource.Path.Should().Be($"/{_sessionsApiName}");
            ApiNames.SessionsResource.Route.Should().Be(_sessionsApiName);
        }
    }
}