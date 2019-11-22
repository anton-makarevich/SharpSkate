using Sanet.SmartSkating.Models;
using Sanet.SmartSkating.Utils;
using Xunit;

namespace Sanet.SmartSkating.Tests.Models
{
    public class RinkTests
    {
        private readonly Coordinate _start = new Coordinate(51.4157028,5.4724154);  // Eindhoven start
        private readonly Coordinate _finish = new Coordinate(51.4148027,5.4724154); // Eindhoven finish   
        private readonly Rink _sut;
        public RinkTests()
        {
            _sut = new Rink(_start,_finish);
        }
        
        [Fact]
        public void CreatesRinkWithStartAndFinishCoordinates()
        {
            Assert.Equal(_start,_sut.Start);
            Assert.Equal(_finish, _sut.Finish);
        }

        [Fact]
        public void FinishLocalIsHundredMetersAwayFromStart()
        {
            var localFinish = _sut.FinishLocal;

            var dist = (localFinish.X,localFinish.Y).GetDistance();
            Assert.Equal(100,dist,0);
        }
    }
}