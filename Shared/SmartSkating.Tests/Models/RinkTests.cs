using System;
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
        public void OneKilometerFinishIsBetweenStartAndFinish()
        {
            var startFinishDx = _sut.FinishLocal.X - _sut.StartLocal.X;
            var startFinishDy = _sut.FinishLocal.Y - _sut.StartLocal.Y;
            
            var startFinish1KDx = _sut.Finish1KLocal.X - _sut.StartLocal.X;
            var startFinish1KDy = _sut.Finish1KLocal.Y - _sut.StartLocal.Y;
            
            Assert.True(Math.Abs(startFinishDx*0.5 - startFinish1KDx) < 0.001);
            Assert.True(Math.Abs(startFinishDy*0.5 - startFinish1KDy) < 0.001);
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
        public void RinkHasFourSectors()
        {
            Assert.Equal(4,_sut.Sectors.Count);
        }
    }
}