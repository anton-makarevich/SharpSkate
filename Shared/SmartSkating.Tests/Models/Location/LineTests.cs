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
    }
}