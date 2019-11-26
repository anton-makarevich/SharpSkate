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
            var dist = (_sut.StartLocal,_sut.FinishLocal).GetDistance();
            Assert.Equal(100,dist,0);
        }

        [Fact]
        public void RinksFirstSectorContainsStart()
        {
            var startLine = _sut.FirstSector.StartLine;
            
            Assert.True(startLine.Contains(_sut.StartLocal));
        }
        
        [Fact]
        public void RinkHasFourSectors()
        {
            Assert.Equal(4,_sut.Sectors.Count);
        }
    }
}