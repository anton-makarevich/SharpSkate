using System;
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
        private readonly Coordinate _firstSectorPoint;
        private readonly Coordinate _fourthSectorPoint;
        private readonly Coordinate _secondSectorPoint;
        private readonly Coordinate _thirdSectorPoint;

        public SessionTests()
        {
            _sut = new Session(_rink);
            _firstSectorPoint = new Coordinate(51.4153197,5.4724154);
            _fourthSectorPoint = new Coordinate(51.4159491,5.4728511);
            _secondSectorPoint = new Coordinate(51.4145113,5.4728282);
            _thirdSectorPoint = new Coordinate(51.4153197,5.4729568);
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

        [Fact]
        public void AddsExtraWayPointWhenCrossingStartFromFourthSectorToFirst()
        {
            _sut.Add(_fourthSectorPoint);
            _sut.Add(_firstSectorPoint);
            
            Assert.Equal(3,_sut.WayPoints.Count);
        }
        
        [Fact]
        public void ExtraWayPointDateIsBetweenPointsDatesItIsInserted()
        {
            var fourthSectorTime = new DateTime(2019,12,13,11,10,9);
            var firstSectorTime = new DateTime(2019,12,13,11,10,19);
            
            _sut.Add(_fourthSectorPoint,fourthSectorTime);
            _sut.Add(_firstSectorPoint, firstSectorTime);

            var extraPoint = _sut.WayPoints[1];
            
            Assert.True(extraPoint.Date>fourthSectorTime);
            Assert.True(extraPoint.Date<firstSectorTime);
        }
        
        [Fact]
        public void AddsExtraWayPointWhenCrossingFinishFromFirstSectorToSecond()
        {
            _sut.Add(_firstSectorPoint);
            _sut.Add(_secondSectorPoint);
            
            Assert.Equal(3,_sut.WayPoints.Count);
        }
        
        [Fact]
        public void AddsExtraWayPointWhenCrossingStart300MFromSecondSectorToThird()
        {
            _sut.Add(_secondSectorPoint);
            _sut.Add(_thirdSectorPoint);
            
            Assert.Equal(3,_sut.WayPoints.Count);
        }
        
        [Fact]
        public void AddsExtraWayPointWhenCrossingStart3KFromThirdSectorToFourth()
        {
            _sut.Add(_thirdSectorPoint);
            _sut.Add(_fourthSectorPoint);
            
            Assert.Equal(3,_sut.WayPoints.Count);
        }
        
        [Fact]
        public void DoesNotAddAnyWayPointsWhenCrossingStartFromFourthSectorToFirst()
        {
            _sut.Add(_firstSectorPoint);
            _sut.Add(_fourthSectorPoint);

            Assert.Single(_sut.WayPoints);
        }
        
        [Fact]
        public void DoesNotAddAnyWayPointsWhenCrossingFinishFromSecondSectorToFirst()
        {
            _sut.Add(_secondSectorPoint);
            _sut.Add(_firstSectorPoint);

            Assert.Single(_sut.WayPoints);
        }
        
        [Fact]
        public void DoesNotAddAnyWayPointsWhenCrossingStart300MFromThirdSectorToSecond()
        {
            _sut.Add(_thirdSectorPoint);
            _sut.Add(_secondSectorPoint);
            
            Assert.Single(_sut.WayPoints);
        }
        
        [Fact]
        public void DoesNotAddAnyWayPointsWhenCrossingStart3KFromFourthSectorToThird()
        {
            _sut.Add(_fourthSectorPoint);
            _sut.Add(_thirdSectorPoint);
            
            Assert.Single(_sut.WayPoints);
        }

        [Fact]
        public void AddsSectorEntryWhenEntersNewSectorInOrder()
        {
            _sut.Add(_firstSectorPoint);
            _sut.Add(_secondSectorPoint);
            
            Assert.NotEmpty(_sut.Sectors);
        }
    }
}