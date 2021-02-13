using System.Linq;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Models.Geometry;
using Sanet.SmartSkating.Models.Location;
using Sanet.SmartSkating.Utils;
using Xunit;

namespace Sanet.SmartSkating.Tests.Models.Geometry
{
    public class RinkTests
    {
        public static Coordinate EindhovenStart = new Coordinate(51.4157028,5.4724154);
        public static Coordinate EindhovenFinish = new Coordinate(51.4148027,5.4724154);

        public static Coordinate GrefrathStart = new Coordinate(51.347566, 6.340406);
        public static Coordinate GrefrathFinish = new Coordinate(51.348305, 6.339573);

        private readonly Rink _sut;
        public RinkTests()
        {
            _sut = new Rink(GrefrathStart, GrefrathFinish, "trackId","RinkName");
        }

        [Fact]
        public void CreatesRinkWithStartAndFinishCoordinates()
        {
            Assert.Equal(GrefrathStart,_sut.Start);
            Assert.Equal(GrefrathFinish, _sut.Finish);
        }

        [Fact]
        public void CreatesRinkWithId()
        {
            Assert.Equal("trackId",_sut.Id);
        }

        [Fact]
        public void CreatesRinkWithName()
        {
            Assert.Equal("RinkName",_sut.Name);
        }

        [Fact]
        public void FinishLocalIsHundredMetersAwayFromStart()
        {
            var dist = (_sut.StartLocal,_sut.FinishLocal).GetDistance();
            Assert.Equal(100,dist,0);
        }

        [Fact]
        public void OneKilometerFinishIsBetweenStartAndFinish()
        {
            var startFinishDx = _sut.FinishLocal.X - _sut.StartLocal.X;
            var startFinishDy = _sut.FinishLocal.Y - _sut.StartLocal.Y;

            var startFinish1KDx = _sut.Finish1KLocal.X - _sut.StartLocal.X;
            var startFinish1KDy = _sut.Finish1KLocal.Y - _sut.StartLocal.Y;

            Assert.Equal(startFinishDx*0.5, startFinish1KDx,0);
            Assert.Equal(startFinishDy*0.5, startFinish1KDy, 0);
        }

        [Fact]
        public void StartTreeHundredIs60MetersAwayFromFinish()
        {
            var dist = (_sut.Start300MLocal,_sut.FinishLocal).GetDistance();
            Assert.Equal(60,dist,0);
        }

        [Fact]
        public void Start3KIs60MetersAwayFromStart()
        {
            var dist = (_sut.Start3KLocal,_sut.StartLocal).GetDistance();
            Assert.Equal(60,dist,0);
        }

        [Fact]
        public void Start3KIs100MetersAwayFromStart300M()
        {
            var dist = (_sut.Start3KLocal,_sut.Start300MLocal).GetDistance();
            Assert.Equal(100,dist,0);
        }

        [Fact]
        public void Start1KIs50MetersAwayFromStart300M()
        {
            var dist = (_sut.Start1KLocal,_sut.Start300MLocal).GetDistance();
            Assert.Equal(50,dist,0);
        }

        [Fact]
        public void Start1KIs50MetersAwayFromStart3K()
        {
            var dist = (_sut.Start1KLocal,_sut.Start3KLocal).GetDistance();
            Assert.Equal(50,dist,0);
        }

        [Fact]
        public void RinkHasFourSectors()
        {
            Assert.Equal(4,_sut.Sectors.Count);
        }

        [Fact]
        public void FirstSectorIsDefined()
        {
            Assert.True(_sut.FirstSector.Corners.Count>0);
        }

        [Fact]
        public void FirstSectorHasCorrectType()
        {
            Assert.Equal(WayPointTypes.FirstSector,_sut.FirstSector.Type);
        }

        [Fact]
        public void RinksFirstSectorContainsStart()
        {
            var startLine = _sut.FirstSector.StartLine;

            Assert.True(startLine.Contains(_sut.StartLocal));
        }

        [Fact]
        public void RinksFirstSectorContainsFinish1K()
        {
            Assert.True(_sut.FirstSector.Contains(_sut.Finish1KLocal));
        }

        [Fact]
        public void RinksFirstSectorContainsFinish()
        {
            var finishLine = _sut.FirstSector.FinishLine;

            Assert.True(finishLine.Contains(_sut.FinishLocal));
        }

        [Fact]
        public void SecondSectorIsDefined()
        {
            Assert.True(_sut.SecondSector.Corners.Count>0);
        }

        [Fact]
        public void SecondSectorHasCorrectType()
        {
            Assert.Equal(WayPointTypes.SecondSector,_sut.SecondSector.Type);
        }

        [Fact]
        public void ThirdSectorIsDefined()
        {
            Assert.True(_sut.ThirdSector.Corners.Count>0);
        }

        [Fact]
        public void ThirdSectorHasCorrectType()
        {
            Assert.Equal(WayPointTypes.ThirdSector,_sut.ThirdSector.Type);
        }

        [Fact]
        public void RinksThirdSectorContainsStart300M()
        {
            var startLine = _sut.ThirdSector.StartLine;

            Assert.True(startLine.Contains(_sut.Start300MLocal));
        }

        [Fact]
        public void RinksThirdSectorContainsStart1K()
        {
            Assert.True(_sut.ThirdSector.Contains(_sut.Start1KLocal));
        }

        [Fact]
        public void RinksThirdSectorContainsStart3K()
        {
            var finishLine = _sut.ThirdSector.FinishLine;

            Assert.True(finishLine.Contains(_sut.Start3KLocal));
        }

        [Fact]
        public void DistanceBetweenFirstSectorsLastPointAndThirdSectorsFirstPointIsLessOrEqualThan80Meters()
        {
            var dist = (_sut.FirstSector.Corners.Last(),_sut.ThirdSector.Corners.First()).GetDistance();
            Assert.True(dist<=80);
        }

        [Fact]
        public void FourthSectorIsDefined()
        {
            Assert.True(_sut.FourthSector.Corners.Count>0);
        }

        [Fact]
        public void FourthSectorHasCorrectType()
        {
            Assert.Equal(WayPointTypes.FourthSector,_sut.FourthSector.Type);
        }

        [Fact]
        public void AllCornersOfSecondSectorAreMoreThan100MAwayFromStart()
        {
            foreach (var distance in _sut.SecondSector.Corners
                .Select(point => (point, _sut.StartLocal).GetDistance()))
            {
                Assert.True(distance>100);
            }
        }

        [Fact]
        public void AllCornersOfFourthSectorAreMoreThan100MAwayFromFinish()
        {
            foreach (var distance in _sut.FourthSector.Corners
                .Select(point => (point, _sut.FinishLocal).GetDistance()))
            {
                Assert.True(distance>100);
            }
        }

        [Fact]
        public void RinkCenterIsAtTheSameDistanceFromAllTheMainPoints()
        {
            var distToStart = (_sut.Center, _sut.StartLocal).GetDistance();
            var distToFinish = (_sut.Center, _sut.FinishLocal).GetDistance();
            var distTo300MStart = (_sut.Center, _sut.Start300MLocal).GetDistance();
            var distTo3KStart = (_sut.Center, _sut.Start3KLocal).GetDistance();

            Assert.Equal(distToStart,distToFinish,0);
            Assert.Equal(distToStart,distTo300MStart,0);
            Assert.Equal(distToStart,distTo3KStart,0);
        }

        [Fact]
        public void ConvertsGeoCoordinateToLocalSystem()
        {
            var localPoint = _sut.ToLocalCoordinateSystem(GrefrathFinish);

            Assert.Equal(-58, localPoint.X,0);
            Assert.Equal(82,localPoint.Y,0);
        }

        [Fact]
        public void ConvertsLocalPointToGeoCoordinate()
        {
            var geoCoordinate = _sut.ToGeoCoordinateSystem(new Point(-58,82));

            Assert.Equal(GrefrathFinish.Latitude, geoCoordinate.Latitude,5);
            Assert.Equal(GrefrathFinish.Longitude, geoCoordinate.Longitude,5);
        }

        [Fact]
        public void AllCornersOfEverySectorAreDifferent()
        {
            foreach (var sector in _sut.Sectors)
            {
                var firstCorner = sector.Corners.First();
                foreach (var corner in sector.Corners.Skip(1))
                {
                    Assert.NotEqual(firstCorner,corner);
                }
            }
        }
    }
}
