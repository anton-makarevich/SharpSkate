using FluentAssertions;
using Sanet.SmartSkating.Dto;
using Xunit;

namespace SmartSkating.Dto.Tests
{
    public class ApiNamesTests
    {
        private const string WayPointsApiName = "waypoints";
        private const string SessionsApiName = "sessions";
        private const string ScansApiName = "scans";
        private const string DevicesApiName = "devices";
        private const string AccountsApiName = "accounts";

        [Fact]
        public void WayPointsApiResourceHasCorrectNames()
        {
            ApiNames.WayPointsResource.Path.Should().Be($"/{WayPointsApiName}");
            ApiNames.WayPointsResource.Route.Should().Be(WayPointsApiName);
        }
        
        [Fact]
        public void SessionsApiResourceHasCorrectName()
        {
            ApiNames.SessionsResource.Path.Should().Be($"/{SessionsApiName}");
            ApiNames.SessionsResource.Route.Should().Be(SessionsApiName);
        }
        
        [Fact]
        public void BleScansApiResourceHasCorrectName()
        {
            ApiNames.BleScansResource.Path.Should().Be($"/{ScansApiName}");
            ApiNames.BleScansResource.Route.Should().Be(ScansApiName);
        }
        
        [Fact]
        public void DevicesApiResourceHasCorrectName()
        {
            ApiNames.DevicesResource.Path.Should().Be($"/{DevicesApiName}");
            ApiNames.DevicesResource.Route.Should().Be(DevicesApiName);
        }
        
        [Fact]
        public void AccountsApiResourceHasCorrectName()
        {
            ApiNames.AccountsResource.Path.Should().Be($"/{AccountsApiName}");
            ApiNames.AccountsResource.Route.Should().Be(AccountsApiName);
        }
    }
}