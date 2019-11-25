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
    }
}