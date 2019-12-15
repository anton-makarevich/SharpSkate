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

            _sut.AddPoint(location, DateTime.Now);
            
            Assert.Empty(_sut.WayPoints);
        }
        
        [Fact]
        public void WayPointIsAddedIfItIsWithinRinkRange()
        {
            var location = new Coordinate(51.4153197,5.4724154);

            _sut.AddPoint(location, DateTime.Now);
            
            Assert.Single(_sut.WayPoints);
        }

        [Fact]
        public void AddedWayPointShouldHaveCorrespondingType()
        {
            var location = new Coordinate(51.4153197,5.4724154);

            _sut.AddPoint(location, DateTime.Now);
            
            Assert.Equal(WayPointTypes.FirstSector,_sut.WayPoints.First().Type);
        }

        [Fact]
        public void PointThatIsNotExactlyInTheSectorShouldBeAdjusted()
        {
            var location = new Coordinate(51.4153197,5.47225);

            _sut.AddPoint(location, DateTime.Now);
            
            Assert.NotEqual(location,_sut.WayPoints.First().AdjustedCoordinate);
        }

        [Fact]
        public void AddsExtraWayPointWhenCrossingStartFromFourthSectorToFirst()
        {
            _sut.AddPoint(_fourthSectorPoint, DateTime.Now);
            _sut.AddPoint(_firstSectorPoint, DateTime.Now);
            
            Assert.Equal(3,_sut.WayPoints.Count);
        }
        
        [Fact]
        public void ExtraWayPointDateIsBetweenPointsDatesItIsInserted()
        {
            var fourthSectorTime = new DateTime(2019,12,13,11,10,9);
            var firstSectorTime = new DateTime(2019,12,13,11,10,19);
            
            _sut.AddPoint(_fourthSectorPoint,fourthSectorTime);
            _sut.AddPoint(_firstSectorPoint, firstSectorTime);

            var extraPoint = _sut.WayPoints[1];
            
            Assert.True(extraPoint.Date>fourthSectorTime);
            Assert.True(extraPoint.Date<firstSectorTime);
        }
        
        [Fact]
        public void AddsExtraWayPointWhenCrossingFinishFromFirstSectorToSecond()
        {
            _sut.AddPoint(_firstSectorPoint, DateTime.Now);
            _sut.AddPoint(_secondSectorPoint, DateTime.Now);
            
            Assert.Equal(3,_sut.WayPoints.Count);
        }
        
        [Fact]
        public void AddsExtraWayPointWhenCrossingStart300MFromSecondSectorToThird()
        {
            _sut.AddPoint(_secondSectorPoint, DateTime.Now);
            _sut.AddPoint(_thirdSectorPoint, DateTime.Now);
            
            Assert.Equal(3,_sut.WayPoints.Count);
        }
        
        [Fact]
        public void AddsExtraWayPointWhenCrossingStart3KFromThirdSectorToFourth()
        {
            _sut.AddPoint(_thirdSectorPoint, DateTime.Now);
            _sut.AddPoint(_fourthSectorPoint, DateTime.Now);
            
            Assert.Equal(3,_sut.WayPoints.Count);
        }
        
        [Fact]
        public void DoesNotAddAnyWayPointsWhenCrossingStartFromFourthSectorToFirst()
        {
            _sut.AddPoint(_firstSectorPoint, DateTime.Now);
            _sut.AddPoint(_fourthSectorPoint, DateTime.Now);

            Assert.Single(_sut.WayPoints);
        }
        
        [Fact]
        public void DoesNotAddAnyWayPointsWhenCrossingFinishFromSecondSectorToFirst()
        {
            _sut.AddPoint(_secondSectorPoint, DateTime.Now);
            _sut.AddPoint(_firstSectorPoint, DateTime.Now);

            Assert.Single(_sut.WayPoints);
        }
        
        [Fact]
        public void DoesNotAddAnyWayPointsWhenCrossingStart300MFromThirdSectorToSecond()
        {
            _sut.AddPoint(_thirdSectorPoint, DateTime.Now);
            _sut.AddPoint(_secondSectorPoint, DateTime.Now);
            
            Assert.Single(_sut.WayPoints);
        }
        
        [Fact]
        public void DoesNotAddAnyWayPointsWhenCrossingStart3KFromFourthSectorToThird()
        {
            _sut.AddPoint(_fourthSectorPoint, DateTime.Now);
            _sut.AddPoint(_thirdSectorPoint, DateTime.Now);
            
            Assert.Single(_sut.WayPoints);
        }

        [Fact]
        public void AddsSectorEntryWhenEntersNewSectorInOrder()
        {
            _sut.AddPoint(_firstSectorPoint, DateTime.Now);
            _sut.AddPoint(_secondSectorPoint, DateTime.Now);
            
            Assert.NotEmpty(_sut.Sectors);
        }

        [Fact]
        public void DoesNotAddSeparatorIfLastPointDoesNotBelongToPreviousSector()
        {
            _sut.AddPoint(_firstSectorPoint, DateTime.Now);
            _sut.AddSeparatingPoint(_rink.Start3K,DateTime.Now.AddSeconds(10), WayPointTypes.Start3K);

            Assert.Single(_sut.WayPoints);
        }

        [Fact]
        public void IncreasesLapCountOnStartCrossing()
        {
            _sut.AddPoint(_firstSectorPoint, DateTime.Now);
            _sut.AddPoint(_secondSectorPoint, DateTime.Now);
            _sut.AddPoint(_thirdSectorPoint, DateTime.Now);
            _sut.AddPoint(_fourthSectorPoint, DateTime.Now);
            _sut.AddPoint(_firstSectorPoint, DateTime.Now);

            Assert.Equal(1,_sut.LapsCount);
        }
        
        [Fact]
        public void CalculatesLastLapTimeOnStartCrossing()
        {
            var firstLapTime = DateTime.Now;
            _sut.AddPoint(_firstSectorPoint, firstLapTime);
            _sut.AddPoint(_secondSectorPoint, firstLapTime.AddSeconds(10));
            _sut.AddPoint(_thirdSectorPoint, firstLapTime.AddSeconds(20));
            _sut.AddPoint(_fourthSectorPoint, firstLapTime.AddSeconds(30));
            _sut.AddPoint(_firstSectorPoint, firstLapTime.AddSeconds(40));

            // 30 is time for middle of fourth sector, 
            // 40 is time for middle of first
            // so we're crossing start in between ~35 seconds
            Assert.Equal(35,_sut.LastLapTime.Seconds); 
        }
    }
}