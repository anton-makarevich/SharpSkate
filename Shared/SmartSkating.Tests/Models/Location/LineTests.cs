using System.Linq;
using Sanet.SmartSkating.Models.Location;
using Xunit;

namespace Sanet.SmartSkating.Tests.Models.Location
{
    public class LineTests
    {
        private readonly Point _firstPoint = new Point(5,0);
        private readonly Point _secondPoint = new Point(10,20);
        
        [Fact]
        public void LineHasCorrectSlopeWhenInitializedWithOnePoint()
        {
            var sut = new Line(_secondPoint);
            
            Assert.Equal(2,sut.Slope,0);
        }
        
        [Fact]
        public void LineHasCorrectSlopeWhenInitializedWithTwoPoint()
        {
            var sut = new Line(_firstPoint, _secondPoint);
            
            Assert.Equal(4,sut.Slope,0);
        }

        [Fact]
        public void InterceptShouldBeZeroForInitializedWithOnePoint()
        {
            var sut = new Line(_secondPoint);
            
            Assert.Equal(0,sut.Intercept,0);
        }
        
        [Fact]
        public void LineHasCorrectInterceptWhenInitializedWithTwoPoint()
        {
            var sut = new Line(_firstPoint, _secondPoint);
            
            Assert.Equal(-20,sut.Intercept,0);
        }

        [Fact]
        public void LengthIsCorrectWhenInitializedWithOnePoint()
        {
            var sut = new Line(_secondPoint);
            
            Assert.Equal(22.36068,sut.Length,5);
        }
        
        [Fact]
        public void LengthIsCorrectWhenInitializedWithTwoPoint()
        {
            var sut = new Line(_firstPoint, _secondPoint);
            
            Assert.Equal(20.61553,sut.Length,5);
        }

        [Fact]
        public void FirstPointIsZeroWhenInitializedWithOnePoint()
        {
            var sut = new Line(_secondPoint);
            
            Assert.Equal(new Point(),sut.Begin);
            Assert.True(sut.Begin.IsZero);
            Assert.Equal(_secondPoint,sut.End);
        }
        
        [Fact]
        public void GetPerpendicularLineToBeginProducesLineWithOnlyBeginPointAndCorrectSlope()
        {
            var secondPoint = new Point(1,1);
            var line = new Line(secondPoint);

            var perpendicularLine = line.GetPerpendicularToBegin();
            
            Assert.Equal(-1,perpendicularLine.Slope,0);
            Assert.Equal(perpendicularLine.Begin,line.Begin);
            Assert.Null(perpendicularLine.End);
        }
        
        [Fact]
        public void GetPerpendicularLineToEndProducesLineWithOnlyBeginPointAndCorrectSlope()
        {
            var secondPoint = new Point(1,1);
            var line = new Line(secondPoint);

            var perpendicularLine = line.GetPerpendicularToEnd();
            
            Assert.NotNull(perpendicularLine);
            Assert.Equal(-1,perpendicularLine.Value.Slope,0);
            Assert.Equal(perpendicularLine.Value.Begin,line.End);
            Assert.Null(perpendicularLine.Value.End);
        }

        [Fact]
        public void GetYCalculatesYForGivenX()
        {
            var secondPoint = new Point(3,4);
            var sut = new Line(secondPoint);

            var y1 = sut.GetY(3);
            var y2 = sut.GetY(6);
            
            Assert.Equal(4,y1);
            Assert.Equal(8,y2);
        }

        [Fact]
        public void FindPointsFromBeginFindsTwoPointsWithCorrectCoordinates()
        {
            var secondPoint = new Point(3,4);
            var sut = new Line(secondPoint);

            var points = sut.FindPointsFromBegin(5).ToList();
            
            Assert.Equal(2,points.Count);
            Assert.Contains(secondPoint,points);
            Assert.Contains(new Point(-3, -4),points);
        }
        
        [Fact]
        public void FindPointsFromEndFindsTwoPointsWithCorrectCoordinatesIfEndIsNotNull()
        {
            var secondPoint = new Point(3,4);
            var sut = new Line(secondPoint);

            var points = sut.FindPointsFromEnd(5).ToList();
            
            Assert.Equal(2,points.Count);
            Assert.Contains(new Point(), points);
            Assert.Contains(new Point(6, 8),points);
        }
    }
}