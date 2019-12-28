using System;
using System.Linq;
using Sanet.SmartSkating.Models;
using Sanet.SmartSkating.Models.Geometry;
using Sanet.SmartSkating.Models.Location;
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
        public void CorrectlyCalculatesSectorTime()
        {
            var fourthSectorTime = DateTime.Now;
            var firstSectorTime = fourthSectorTime.AddSeconds(10);
            var secondSectorTime = fourthSectorTime.AddSeconds(20);
            
            _sut.AddPoint(_fourthSectorPoint, fourthSectorTime);
            _sut.AddPoint(_firstSectorPoint, firstSectorTime);
            _sut.AddPoint(_secondSectorPoint, secondSectorTime);
            
            Assert.Equal(9, _sut.Sectors.Last().Time.Seconds);
        }
        
        [Fact]
        public void CorrectlyCalculatesFirstSectorTime()
        {
            var firstSectorEnterTime = DateTime.Now;
            var firstSectorTime = firstSectorEnterTime.AddSeconds(10);
            var secondSectorTime = firstSectorEnterTime.AddSeconds(20);
            
            _sut.AddPoint(_firstSectorPoint, firstSectorEnterTime);
            _sut.AddPoint(_firstSectorPoint, firstSectorTime);
            _sut.AddPoint(_secondSectorPoint, secondSectorTime);
            
            Assert.Equal(15, _sut.Sectors.Last().Time.Seconds);
        }

        [Fact]
        public void UpdatesBestSectorWhenFirstSectorIsAdded()
        {
            _sut.AddPoint(_firstSectorPoint, DateTime.Now);
            _sut.AddPoint(_secondSectorPoint, DateTime.Now);
            
            Assert.NotNull(_sut.BestSector);
        }
        
        [Fact]
        public void UpdatesBestSectorWhenNewSectorIsBetter()
        {
            _sut.AddPoint(_firstSectorPoint, DateTime.Now);
            _sut.AddPoint(_secondSectorPoint, DateTime.Now.AddSeconds(20));
            _sut.AddPoint(_thirdSectorPoint, DateTime.Now.AddSeconds(10));

            Assert.NotNull(_sut.BestSector);
            Assert.Equal(WayPointTypes.SecondSector, _sut.BestSector.Value.Type);
        }
        
        [Fact]
        public void DoesNotUpdateBestSectorWhenNewSectorIsBetter()
        {
            _sut.AddPoint(_firstSectorPoint, DateTime.Now);
            _sut.AddPoint(_secondSectorPoint, DateTime.Now.AddSeconds(20));
            _sut.AddPoint(_thirdSectorPoint, DateTime.Now.AddSeconds(40));

            Assert.NotNull(_sut.BestSector);
            Assert.Equal(WayPointTypes.FirstSector, _sut.BestSector.Value.Type);
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
            AddLap(firstLapTime, 10);

            // 30 is time for middle of fourth sector, 
            // 40 is time for middle of first
            // so we're crossing start in between ~35 seconds
            Assert.Equal(35,_sut.LastLapTime.Seconds); 
        }
        
        [Fact]
        public void UpdatesBestLastLapTimeOnStartCrossing()
        {
            var firstLapTime = DateTime.Now;
            AddLap(firstLapTime, 10);

            Assert.Equal(35,_sut.BestLapTime.Seconds); 
        }
        
        [Fact]
        public void UpdatesBestLastLapTimeWhenNewLapIsBetter()
        {
            var firstLapTime = DateTime.Now;
            AddLap(firstLapTime,10);
            AddLap(firstLapTime.AddSeconds(40),8);
        
            Assert.Equal(32,_sut.BestLapTime.Seconds); 
        }
        
        [Fact]
        public void DoesNotUpdateBestLastLapTimeWhenNewLapIsSlower()
        {
            var firstLapTime = DateTime.Now;
            AddLap(firstLapTime,10);
            AddLap(firstLapTime.AddSeconds(40),12);
        
            Assert.Equal(35,_sut.BestLapTime.Seconds); 
        }

        private void AddLap(DateTime startTime, int sectorTime)
        {
            _sut.AddPoint(_firstSectorPoint, startTime);
            _sut.AddPoint(_secondSectorPoint, startTime.AddSeconds(sectorTime));
            _sut.AddPoint(_thirdSectorPoint, startTime.AddSeconds(sectorTime*2));
            _sut.AddPoint(_fourthSectorPoint, startTime.AddSeconds(sectorTime*3));
            _sut.AddPoint(_firstSectorPoint, startTime.AddSeconds(sectorTime*4));
        }

        [Fact]
        public void SetsStartTime()
        {
            var startTime = DateTime.Now;
            
            _sut.SetStartTime(startTime);
            
            Assert.Equal(startTime,_sut.StartTime);
        }
    }
}