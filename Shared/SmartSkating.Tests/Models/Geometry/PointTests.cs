using Sanet.SmartSkating.Models.Geometry;
using Xunit;

namespace Sanet.SmartSkating.Tests.Models.Geometry
{
    public class PointTests
    {
        [Fact]
        public void PointInitializedWithoutParametersIsInBeginningOfCoordinates()
        {
            var sut = new Point();
            
            Assert.Equal(0,sut.X);
            Assert.Equal(0,sut.Y);
        }

        [Fact]
        public void IsZeroIsTrueForPointInBeginningOfCoordinates()
        {
            var sut = new Point();
            
            Assert.True(sut.IsZero);
        }
    }
}