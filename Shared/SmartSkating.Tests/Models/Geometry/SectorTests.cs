using System;
using Sanet.SmartSkating.Models.Geometry;
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
            
            var sut = new Sector(startPoints,finishPoints);
            
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
                var _ = new Sector(startPoints, finishPoints);
            });
        }   
        
        [Fact]
        public void ConstructorThrowsArgumentExceptionIfLessThanTwoFinishPointsWerePassed()
        {
            var startPoints = new[] {new Point(), new Point(2, 1)};
            var finishPoints = new[] {new Point(1, -1)};
            Assert.Throws<ArgumentException>(()=>
            {
                var _ = new Sector(startPoints, finishPoints);
            });
        }  
        
        [Fact]
        public void SectorsFinishLineContainsFinishPoints()
        {
            var startPoints = new[] {new Point(), new Point(1, 1)};
            var finishPoints = new[] {new Point(1, -1), new Point(2, 0)};
            
            var sut = new Sector(startPoints,finishPoints);
            
            Assert.True(sut.FinishLine.Contains(finishPoints[0]));
            Assert.True(sut.FinishLine.Contains(finishPoints[1]));
        }

        [Fact]
        public void SectorHasFourCorners()
        {
            var startPoints = new[] {new Point(), new Point(1, 1)};
            var finishPoints = new[] {new Point(1, -1), new Point(2, 0)};
            
            var sut = new Sector(startPoints,finishPoints);
            
            Assert.Equal(4,sut.Corners.Count);
        }
        
        [Fact]
        public void SectorContainsPointThatIsInsideItsCorners()
        {
            var startPoints = new[] {new Point(), new Point(1, 1)};
            var finishPoints = new[] {new Point(1, -1), new Point(2, 0)};
            var testPoint = new Point(1.2,0.2);
            
            var sut = new Sector(startPoints,finishPoints);
            
            Assert.True(sut.Contains(testPoint));
        }

        [Fact]
        public void SectorsCenterIsInsideSector()
        {
            var startPoints = new[] {new Point(), new Point(1, 1)};
            var finishPoints = new[] {new Point(1, -1), new Point(2, 0)};
            
            var sut = new Sector(startPoints,finishPoints);
            Assert.True(sut.Contains(sut.Center));
        }
        
        [Fact]
        public void SectorsCenterIsCalculatedCorrectly()
        {
            var startPoints = new[] {new Point(), new Point(1, 1)};
            var finishPoints = new[] {new Point(1, -1), new Point(2, 0)};
            
            var sut = new Sector(startPoints,finishPoints);
            
            Assert.Equal(new Point(1,0),sut.Center);
        }
    }
}