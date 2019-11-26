using Sanet.SmartSkating.Models;
using Sanet.SmartSkating.Models.Location;
using Xunit;

namespace Sanet.SmartSkating.Tests.Models
{
    public class SectorTests
    {
        [Fact]
        public void SectorsStartLineContainsStartPoints()
        {
            var startPoints = new[] {new Point(), new Point(1, 1)};
            var finishPoints = new[] {new Point(1, -1), new Point(2, 1)};
            
            var sut = new Sector(startPoints,finishPoints);
            
            Assert.True(sut.StartLine.Contains(startPoints[0]));
            Assert.True(sut.StartLine.Contains(startPoints[1]));
        }
    }
}