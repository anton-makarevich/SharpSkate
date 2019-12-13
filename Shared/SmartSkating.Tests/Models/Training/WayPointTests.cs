using System;
using Sanet.SmartSkating.Models;
using Sanet.SmartSkating.Models.Training;
using Xunit;

namespace Sanet.SmartSkating.Tests.Models.Training
{
    public class WayPointTests
    {
        [Fact]
        public void AdjustedCoordinateIsTheSameAsOriginalIfNotProvided()
        {
            var coordinate = new Coordinate(10,10);
            
            var sut = new WayPoint(coordinate,DateTime.Now);
            
            Assert.Equal(sut.OriginalCoordinate,sut.AdjustedCoordinate);
        }
        
        [Fact]
        public void TypeIsUnknownIfNotProvided()
        {
            var coordinate = new Coordinate(10,10);
            
            var sut = new WayPoint(coordinate,DateTime.Now);
            
            Assert.Equal(WayPointTypes.Unknown,sut.Type);
        }
    }
}