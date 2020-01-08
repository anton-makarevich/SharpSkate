using System;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Models.Geometry;
using Sanet.SmartSkating.Models.Training;
using Sanet.SmartSkating.Utils;
using Xunit;

namespace Sanet.SmartSkating.Tests.Models.Geometry
{
    public class SectorTests
    {
        [Fact]
        public void SectorsStartLineContainsStartPoints()
        {
            var startPoints = new[] {new Point(), new Point(1, 1)};
            var finishPoints = new[] {new Point(1, -1), new Point(2, 1)};
            
            var sut = new Sector(startPoints,finishPoints, WayPointTypes.Unknown);
            
            Assert.True(sut.StartLine.Contains(startPoints[0]));
            Assert.True(sut.StartLine.Contains(startPoints[1]));
        }

        [Fact]
        public void ConstructorThrowsArgumentExceptionIfLessThanTwoStartPointsWerePassed()
        {
            var startPoints = new[] {new Point()};
            var finishPoints = new[] {new Point(1, -1), new Point(2, 1)};
            Assert.Throws<ArgumentException>(()=>
            {
                var _ = new Sector(startPoints, finishPoints, WayPointTypes.Unknown);
            });
        }   
        
        [Fact]
        public void ConstructorThrowsArgumentExceptionIfLessThanTwoFinishPointsWerePassed()
        {
            var startPoints = new[] {new Point(), new Point(2, 1)};
            var finishPoints = new[] {new Point(1, -1)};
            Assert.Throws<ArgumentException>(()=>
            {
                var _ = new Sector(startPoints, finishPoints, WayPointTypes.Unknown);
            });
        }  
        
        [Fact]
        public void SectorsFinishLineContainsFinishPoints()
        {
            var startPoints = new[] {new Point(), new Point(1, 1)};
            var finishPoints = new[] {new Point(1, -1), new Point(2, 0)};
            
            var sut = new Sector(startPoints,finishPoints, WayPointTypes.Unknown);
            
            Assert.True(sut.FinishLine.Contains(finishPoints[0]));
            Assert.True(sut.FinishLine.Contains(finishPoints[1]));
        }

        [Fact]
        public void SectorHasFourCorners()
        {
            var startPoints = new[] {new Point(), new Point(1, 1)};
            var finishPoints = new[] {new Point(1, -1), new Point(2, 0)};
            
            var sut = new Sector(startPoints,finishPoints, WayPointTypes.Unknown);
            
            Assert.Equal(4,sut.Corners.Count);
        }
        
        [Fact]
        public void SectorContainsPointThatIsInsideItsCorners()
        {
            var startPoints = new[] {new Point(), new Point(1, 1)};
            var finishPoints = new[] {new Point(1, -1), new Point(2, 0)};
            var testPoint = new Point(1.2,0.2);
            
            var sut = new Sector(startPoints,finishPoints, WayPointTypes.Unknown);
            
            Assert.True(sut.Contains(testPoint));
        }

        [Fact]
        public void SectorsCenterIsInsideSector()
        {
            var startPoints = new[] {new Point(), new Point(1, 1)};
            var finishPoints = new[] {new Point(1, -1), new Point(2, 0)};
            
            var sut = new Sector(startPoints,finishPoints, WayPointTypes.Unknown);
            Assert.True(sut.Contains(sut.Center));
        }
        
        [Fact]
        public void SectorsCenterIsCalculatedCorrectly()
        {
            var startPoints = new[] {new Point(), new Point(1, 1)};
            var finishPoints = new[] {new Point(1, -1), new Point(2, 0)};
            
            var sut = new Sector(startPoints,finishPoints, WayPointTypes.Unknown);
            
            Assert.Equal(new Point(1,0),sut.Center);
        }
        
        [Fact]
        public void FindsIntersectionOfLineWithSector()
        {
            var sut = GetSut();
            var line = new Line(new Point(1,3),new Point(3,1));

            var intersection = sut.FindIntersection(line);
            
            Assert.Equal(new Point(2,2),intersection);
        }
        
        [Fact]
        public void DoesNotFindIntersectionOfLineWithSectorIfLineIsInside()
        {
            var sut = GetSut();
            var line = new Line(new Point(0,2),new Point(5,-3));

            var intersection = sut.FindIntersection(line);
            
            Assert.Null(intersection);
        }
        
        [Fact]
        public void DoesNotFindIntersectionOfLineWithSectorIfBothLinePointsAreOutside()
        {
            var sut = GetSut();
            var line = new Line(new Point(1,0),new Point(5,0));

            var intersection = sut.FindIntersection(line);
            
            Assert.Null(intersection);
        }

        private Sector GetSut()
        {
            var startPoints = new[] {new Point(), new Point(3, 3)};
            var finishPoints = new[] {new Point(0, -3), new Point(6, 0)};
            
            return new Sector(startPoints,finishPoints, WayPointTypes.Unknown);
        }
    }
}