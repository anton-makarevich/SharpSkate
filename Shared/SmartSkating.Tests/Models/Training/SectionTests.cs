using System;
using Sanet.SmartSkating.Models;
using Sanet.SmartSkating.Models.Training;
using Xunit;

namespace Sanet.SmartSkating.Tests.Models.Training
{
    public class SectionTests
    {
        private readonly DateTime _startTime;
        private readonly DateTime _finishTime;
        private readonly Coordinate _location;

        public SectionTests()
        {
            _startTime = new DateTime(2019,12,13,11,10,9);
            _finishTime = new DateTime(2019,12,13,11,10,19);
            _location = new Coordinate();
        }

        [Fact]
        public void SectionFromStartToFinishIsFirstSector()
        {
            const WayPointTypes statType = WayPointTypes.Start;
            const WayPointTypes finishType = WayPointTypes.Finish;
            
            var startWayPoint = new WayPoint(
                _location,
                _location,
                _startTime,
                statType);
            
            var finishWayPoint = new WayPoint(
                _location,
                _location,
                _finishTime,
                finishType);
            
            var sut = new Section(startWayPoint,finishWayPoint);
            
            Assert.Equal(WayPointTypes.FirstSector, sut.Type);
        }
    }
}