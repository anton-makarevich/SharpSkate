using Sanet.SmartSkating.Models;
using Xunit;

namespace Sanet.SmartSkating.Tests.Models
{
    public class RinkTests
    {
        [Fact]
        public void CreatesRinkWithStartAndFinishCoordinates()
        {
            var start = new Coordinate(51.4157028,5.4724154);  // Eindhoven start
            var finish = new Coordinate(51.4147827,5.4723886); // Eindhoven finish

            var sut = new Rink(start,finish);
            
            Assert.Equal(start,sut.Start);
            Assert.Equal(finish, sut.Finish);
        }
    }
}