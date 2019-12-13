using Sanet.SmartSkating.Models.Training;
using Xunit;

namespace Sanet.SmartSkating.Tests.Models.Training
{
    public class WayPointsTypesTests
    {
        [Fact]
        public void FirstSectorIsBetweenStartAndFinish()
        {
            var result = (WayPointTypes.Start, WayPointTypes.Finish)
                .GetSectorTypeBetween();
            
            Assert.Equal(WayPointTypes.FirstSector, result);
        }
        
        [Fact]
        public void SecondSectorIsBetweenFinishAndStart300M()
        {
            var result = (WayPointTypes.Finish, WayPointTypes.Start300M)
                .GetSectorTypeBetween();
            
            Assert.Equal(WayPointTypes.SecondSector, result);
        }
        
        [Fact]
        public void ThirdSectorIsBetweenStart300MAndStart3K()
        {
            var result = (WayPointTypes.Start300M, WayPointTypes.Start3K)
                .GetSectorTypeBetween();
            
            Assert.Equal(WayPointTypes.ThirdSector, result);
        }
        
        [Fact]
        public void FourthSectorIsBetweenStart3KAndStart()
        {
            var result = (WayPointTypes.Start3K, WayPointTypes.Start)
                .GetSectorTypeBetween();
            
            Assert.Equal(WayPointTypes.FourthSector, result);
        }

        [Fact]
        public void StartSeparatesFourthAndFirstSectors()
        {
            var result = (WayPointTypes.FourthSector, WayPointTypes.FirstSector)
                .GetTypeSeparatingSectors();
            
            Assert.Equal(WayPointTypes.Start, result);
        }
        
        [Fact]
        public void FinishSeparatesFirstAndSecondSectors()
        {
            var result = (WayPointTypes.FirstSector, WayPointTypes.SecondSector)
                .GetTypeSeparatingSectors();
            
            Assert.Equal(WayPointTypes.Finish, result);
        }
        
        [Fact]
        public void Start300MSeparatesSecondAndThirdSectors()
        {
            var result = (WayPointTypes.SecondSector, WayPointTypes.ThirdSector)
                .GetTypeSeparatingSectors();
            
            Assert.Equal(WayPointTypes.Start300M, result);
        }
        
        [Fact]
        public void Start3KSeparatesThirdAndFourthSectors()
        {
            var result = (WayPointTypes.ThirdSector, WayPointTypes.FourthSector)
                .GetTypeSeparatingSectors();
            
            Assert.Equal(WayPointTypes.Start3K, result);
        }
    }
}