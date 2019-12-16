using System;
using Sanet.SmartSkating.Models;
using Sanet.SmartSkating.Models.Training;
using Xunit;

namespace Sanet.SmartSkating.Tests.Models.Training
{
    public class SectionTests
    {
        private readonly Section _sut;

        private const WayPointTypes StatType = WayPointTypes.Start;
        private const WayPointTypes FinishType = WayPointTypes.Finish;

        public SectionTests()
        {
            var startTime = new DateTime(2019,12,13,11,10,9);
            var finishTime = new DateTime(2019,12,13,11,10,19);
            var location = new Coordinate();
            var startWayPoint = new WayPoint(
                location,
                location,
                startTime,
                StatType);
            var finishWayPoint = new WayPoint(
                location,
                location,
                finishTime,
                FinishType);
            _sut = new Section(startWayPoint,finishWayPoint);
        }

        [Fact]
        public void SectionFromStartToFinishIsFirstSector()
        {
            Assert.Equal(WayPointTypes.FirstSector, _sut.Type);
        }

        [Fact]
        public void SectionHasTimeFromStartToFinish()
        {
            Assert.Equal(10,_sut.Time.Seconds);
        }
    }
}