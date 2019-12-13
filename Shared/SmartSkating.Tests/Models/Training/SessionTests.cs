using System.Linq;
using Sanet.SmartSkating.Models;
using Sanet.SmartSkating.Models.Geometry;
using Sanet.SmartSkating.Models.Training;
using Sanet.SmartSkating.Tests.Models.Geometry;
using Xunit;

namespace Sanet.SmartSkating.Tests.Models.Training
{
    public class SessionTests
    {
        private readonly Rink _rink = new Rink(RinkTests.EindhovenStart,RinkTests.EindhovenFinish);
        
        private readonly Session _sut;

        public SessionTests()
        {
            _sut = new Session(_rink);
        }

        [Fact]
        public void WayPointIsNotAddedIfItIsOutOfRinkRange()
        {
            var location = new Coordinate(51.4159838,5.4709491);

            _sut.Add(location);
            
            Assert.Empty(_sut.WayPoints);
        }
        
        [Fact]
        public void WayPointIsAddedIfItIsWithinRinkRange()
        {
            var location = new Coordinate(51.4153197,5.4724154);

            _sut.Add(location);
            
            Assert.Single(_sut.WayPoints);
        }

        [Fact]
        public void AddedWayPointShouldHaveCorrespondingType()
        {
            var location = new Coordinate(51.4153197,5.4724154);

            _sut.Add(location);
            
            Assert.Equal(WayPointTypes.FirstSector,_sut.WayPoints.First().Type);
        }

        [Fact]
        public void PointThatIsNotExactlyInTheSectorShouldBeAdjusted()
        {
            var location = new Coordinate(51.4153197,5.47230);

            _sut.Add(location);
            
            Assert.NotEqual(location,_sut.WayPoints.First().AdjustedCoordinate);
        }
    }
}