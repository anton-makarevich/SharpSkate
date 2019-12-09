using Sanet.SmartSkating.Models.Location;
using Sanet.SmartSkating.Utils;
using Xunit;

namespace Sanet.SmartSkating.Tests.Utils
{
    public class MathExtensionsTests
    {
        [Fact]
        public void ConvertsDegreesToRadians()
        {
            const double valueInDegrees = 45;

            var result = valueInDegrees.ToRadians();
            
            Assert.Equal(0.785398,result,6);
        }

        [Fact]
        public void CalculatesDistanceByCoordinateDeltas()
        {
            const double dX = 3;
            const double dY = 4;

            var result = (dX, dY).GetDistance();
            
            Assert.Equal(5,result,0);
        }
        
        [Fact]
        public void CalculatesDistanceBetweenPoints()
        {
            var firsPoint = new Point();
            var secondPoint = new Point(3,4);

            var result = (firsPoint, secondPoint).GetDistance();
            
            Assert.Equal(5,result,0);
        }
        
        [Fact]
        public void LineContainsReturnsTrueIfPointBelongsToLine()
        {
            var secondPoint = new Point(2,2);
            
            var sut = new Line(secondPoint);
            
            Assert.True(sut.Contains(new Point(1,1)));
        }
        
        [Fact]
        public void LineContainsReturnsFalseIfPointDoesNotBelongToLine()
        {
            var secondPoint = new Point(2,2);
            
            var sut = new Line(secondPoint);
            
            Assert.False(sut.Contains(new Point(1,2)));
        }
        
        [Fact]
        public void LineContainsReturnsCorrectResultForVerticalLine()
        {
            var firstPoint = new Point(2,0);
            var secondPoint = new Point(2,2);
            
            var sut = new Line(firstPoint,secondPoint);
            
            Assert.True(sut.Contains(new Point(2,1)));
        }

        [Fact]
        public void DetectsPointsInsideThePolygon()
        {
            var polygon = new[]
            {
                new Point(0, 2),
                new Point(4, 4),
                new Point(4, 1),
                new Point(1, -1),
            };
            
            var inPoints = new[]
            {
                new Point(1, 1),
                new Point(2, 2),
                new Point(3, 3),
            };

            foreach (var inPoint in inPoints)
            {
                Assert.True(inPoint.IsInPolygon(polygon));
            }
        }
        
        [Fact]
        public void DetectsPointsOnThePolygon()
        {
            var polygon = new[]
            {
                new Point(0, 2),
                new Point(4, 4),
                new Point(4, 1),
                new Point(1, -1),
            };
            
            var onPoints = new[]
            {
                new Point(0, 2),
                new Point(4, 4),
                new Point(2, 3),
            };

            foreach (var onPoint in onPoints)
            {
                Assert.True(onPoint.IsInPolygon(polygon));
            }
        }
        
        [Fact]
        public void DoesNotDetectPointsOutsideThePolygon()
        {
            var polygon = new[]
            {
                new Point(0, 2),
                new Point(4, 4),
                new Point(4, 1),
                new Point(1, -1),
            };
            
            var outPoints = new[]
            {
                new Point(0, 0),
                new Point(4, 0),
                new Point(1, 3),
            };

            foreach (var outPoint in outPoints)
            {
                Assert.False(outPoint.IsInPolygon(polygon));
            }
        }
    }
}