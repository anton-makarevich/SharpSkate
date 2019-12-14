using System;
using Sanet.SmartSkating.Models.Geometry;
using Sanet.SmartSkating.Models.Training;
using Sanet.SmartSkating.Tests.Models.Geometry;
using Xunit;

namespace Sanet.SmartSkating.Tests.Models.Training
{
    public class WayPointsTypesTests
    {
        private readonly Rink _rink = new Rink(RinkTests.EindhovenStart,RinkTests.EindhovenFinish);
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

        [Fact]
        public void ReturnsCorrectLocationForStart()
        {
            var result = WayPointTypes.Start.GetSeparatingPointLocationForType(_rink);
            
            Assert.Equal(_rink.Start,result);
        }
        
        [Fact]
        public void ReturnsCorrectLocationForFinish()
        {
            var result = WayPointTypes.Finish.GetSeparatingPointLocationForType(_rink);
            
            Assert.Equal(_rink.Finish,result);
        }
        
        [Fact]
        public void ReturnsCorrectLocationForStart300M()
        {
            var result = WayPointTypes.Start300M.GetSeparatingPointLocationForType(_rink);
            
            Assert.Equal(_rink.Start300M,result);
        }
        
        [Fact]
        public void ReturnsCorrectLocationForStart3K()
        {
            var result = WayPointTypes.Start3K.GetSeparatingPointLocationForType(_rink);
            
            Assert.Equal(_rink.Start3K,result);
        }
        
        [Fact]
        public void ThrowExceptionForUnsupportedType()
        {
            Assert.Throws<NotSupportedException>(() => WayPointTypes.FirstSector.GetSeparatingPointLocationForType(_rink));
        }

        [Fact]
        public void StartIsPreviousSeparationPointToFinish()
        {
            var result = WayPointTypes.Finish.GetPreviousSeparationPointType();
            
            Assert.Equal(WayPointTypes.Start,result);
        }

        [Fact]
        public void FinishIsPreviousSeparationPointToStart300M()
        {
            var result = WayPointTypes.Start300M.GetPreviousSeparationPointType();
            
            Assert.Equal(WayPointTypes.Finish,result);
        }
        
        [Fact]
        public void Start300MIsPreviousSeparationPointToStart3K()
        {
            var result = WayPointTypes.Start3K.GetPreviousSeparationPointType();
            
            Assert.Equal(WayPointTypes.Start300M,result);
        }
        
        [Fact]
        public void Start3KIsPreviousSeparationPointToStart()
        {
            var result = WayPointTypes.Start.GetPreviousSeparationPointType();
            
            Assert.Equal(WayPointTypes.Start3K,result);
        }
        
        [Fact]
        public void StartIsPreviousSeparationPointForFirstSection()
        {
            var result = WayPointTypes.FirstSector.GetPreviousSeparationPointType();
            
            Assert.Equal(WayPointTypes.Start,result);
        }

        [Fact]
        public void FinishIsPreviousSeparationPointForSecondSector()
        {
            var result = WayPointTypes.SecondSector.GetPreviousSeparationPointType();
            
            Assert.Equal(WayPointTypes.Finish,result);
        }
        
        [Fact]
        public void Start300MIsPreviousSeparationPointForThirdSector()
        {
            var result = WayPointTypes.ThirdSector.GetPreviousSeparationPointType();
            
            Assert.Equal(WayPointTypes.Start300M,result);
        }
        
        [Fact]
        public void Start3KIsPreviousSeparationPointForFourthSector()
        {
            var result = WayPointTypes.FourthSector.GetPreviousSeparationPointType();
            
            Assert.Equal(WayPointTypes.Start3K,result);
        }

        [Fact]
        public void FourthSectorIsBeforeStart()
        {
            var result = WayPointTypes.Start.GetPreviousSectorType();
            
            Assert.Equal(WayPointTypes.FourthSector,result);
        }
        
        [Fact]
        public void FourthSectorIsBeforeFirstSector()
        {
            var result = WayPointTypes.FirstSector.GetPreviousSectorType();
            
            Assert.Equal(WayPointTypes.FourthSector,result);
        }
        
        [Fact]
        public void FirstSectorIsBeforeFinish()
        {
            var result = WayPointTypes.Finish.GetPreviousSectorType();
            
            Assert.Equal(WayPointTypes.FirstSector,result);
        }
        
        [Fact]
        public void FirstSectorIsBeforeSecondSector()
        {
            var result = WayPointTypes.SecondSector.GetPreviousSectorType();
            
            Assert.Equal(WayPointTypes.FirstSector,result);
        }
        
        [Fact]
        public void SecondSectorIsBeforeStart300M()
        {
            var result = WayPointTypes.Start300M.GetPreviousSectorType();
            
            Assert.Equal(WayPointTypes.SecondSector,result);
        }
        
        [Fact]
        public void SecondSectorIsBeforeThirdSector()
        {
            var result = WayPointTypes.ThirdSector.GetPreviousSectorType();
            
            Assert.Equal(WayPointTypes.SecondSector,result);
        }
        
        [Fact]
        public void ThirdSectorIsBeforeStart3K()
        {
            var result = WayPointTypes.Start3K.GetPreviousSectorType();
            
            Assert.Equal(WayPointTypes.ThirdSector,result);
        }
        
        [Fact]
        public void ThirdSectorIsBeforeFourthSector()
        {
            var result = WayPointTypes.FourthSector.GetPreviousSectorType();
            
            Assert.Equal(WayPointTypes.ThirdSector,result);
        }
    }
}