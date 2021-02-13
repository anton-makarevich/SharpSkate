using System;
using FluentAssertions;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Models.Geometry;
using Sanet.SmartSkating.Models.Training;
using Sanet.SmartSkating.Tests.Models.Geometry;
using Xunit;

namespace Sanet.SmartSkating.Tests.Models.Training
{
    public class WayPointsTypesTests
    {
        private readonly Rink _rink = new Rink(RinkTests.EindhovenStart,RinkTests.EindhovenFinish,"RinkId");

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
            var (point, coordinate) = WayPointTypes.Start.GetSeparatingPointLocationForType(_rink);

            point.Should().Be(_rink.StartLocal);
            coordinate.Should().Be(_rink.Start);
        }

        [Fact]
        public void ReturnsCorrectLocationForFinish()
        {
            var (point, coordinate) = WayPointTypes.Finish.GetSeparatingPointLocationForType(_rink);

            point.Should().Be(_rink.FinishLocal);
            coordinate.Should().Be(_rink.Finish);
        }

        [Fact]
        public void ReturnsCorrectLocationForStart300M()
        {
            var (point, coordinate) = WayPointTypes.Start300M.GetSeparatingPointLocationForType(_rink);

            point.Should().Be(_rink.Start300MLocal);
            coordinate.Should().Be(_rink.Start300M);
        }

        [Fact]
        public void ReturnsCorrectLocationForStart3K()
        {
            var (point, coordinate) = WayPointTypes.Start3K.GetSeparatingPointLocationForType(_rink);

           point.Should().Be(_rink.Start3KLocal);
           coordinate.Should().Be(_rink.Start3K);
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

        [Fact]
        public void CorrectlyConvertsFirstSectorTypeToString()
        {
            var result = WayPointTypes.FirstSector.GetSectorName();

            Assert.Equal("1st",result);
        }

        [Fact]
        public void CorrectlyConvertsSecondSectorTypeToString()
        {
            var result = WayPointTypes.SecondSector.GetSectorName();

            Assert.Equal("2nd",result);
        }

        [Fact]
        public void CorrectlyConvertsThirdSectorTypeToString()
        {
            var result = WayPointTypes.ThirdSector.GetSectorName();

            Assert.Equal("3rd",result);
        }

        [Fact]
        public void CorrectlyConvertsFourthSectorTypeToString()
        {
            var result = WayPointTypes.FourthSector.GetSectorName();

            Assert.Equal("4th",result);
        }

        [Fact]
        public void ReturnsNaForSectorNameIfTypeIsNotSector()
        {
            var result = WayPointTypes.Start.GetSectorName();

            Assert.Equal("NA",result);
        }

        [Fact]
        public void FourthSectorIsAfterStart3K()
        {
            var result = WayPointTypes.Start3K.GetNextSectorType();

            Assert.Equal(WayPointTypes.FourthSector,result);
        }

        [Fact]
        public void FourthSectorIsAfterThirdSector()
        {
            var result = WayPointTypes.ThirdSector.GetNextSectorType();

            Assert.Equal(WayPointTypes.FourthSector,result);
        }

        [Fact]
        public void FirstSectorIsAfterStart()
        {
            var result = WayPointTypes.Start.GetNextSectorType();

            Assert.Equal(WayPointTypes.FirstSector,result);
        }

        [Fact]
        public void FirstSectorIsAfterFourthSector()
        {
            var result = WayPointTypes.FourthSector.GetNextSectorType();

            Assert.Equal(WayPointTypes.FirstSector,result);
        }

        [Fact]
        public void SecondSectorIsAfterFinish()
        {
            var result = WayPointTypes.Finish.GetNextSectorType();

            Assert.Equal(WayPointTypes.SecondSector,result);
        }

        [Fact]
        public void SecondSectorIsAfterFirstSector()
        {
            var result = WayPointTypes.FirstSector.GetNextSectorType();

            Assert.Equal(WayPointTypes.SecondSector,result);
        }

        [Fact]
        public void ThirdSectorIsAfterStart300M()
        {
            var result = WayPointTypes.Start300M.GetNextSectorType();

            Assert.Equal(WayPointTypes.ThirdSector,result);
        }

        [Fact]
        public void ThirdSectorIsAfterSecondSector()
        {
            var result = WayPointTypes.SecondSector.GetNextSectorType();

            Assert.Equal(WayPointTypes.ThirdSector,result);
        }

        [Fact]
        public void Start300MIsNextSeparationPointToFinish()
        {
            var result = WayPointTypes.Finish.GetNextSeparationPointType();

            Assert.Equal(WayPointTypes.Start300M,result);
        }

        [Fact]
        public void Start3KIsNextSeparationPointToStart300M()
        {
            var result = WayPointTypes.Start300M.GetNextSeparationPointType();

            Assert.Equal(WayPointTypes.Start3K,result);
        }

        [Fact]
        public void StartIsNextSeparationPointToStart3K()
        {
            var result = WayPointTypes.Start3K.GetNextSeparationPointType();

            Assert.Equal(WayPointTypes.Start,result);
        }

        [Fact]
        public void FinishIsNextSeparationPointToStart()
        {
            var result = WayPointTypes.Start.GetNextSeparationPointType();

            Assert.Equal(WayPointTypes.Finish,result);
        }

        [Fact]
        public void FinishIsNextSeparationPointForFirstSection()
        {
            var result = WayPointTypes.FirstSector.GetNextSeparationPointType();

            Assert.Equal(WayPointTypes.Finish,result);
        }

        [Fact]
        public void Start300MIsNextSeparationPointForSecondSector()
        {
            var result = WayPointTypes.SecondSector.GetNextSeparationPointType();

            Assert.Equal(WayPointTypes.Start300M,result);
        }

        [Fact]
        public void Start3KIsNextSeparationPointForThirdSector()
        {
            var result = WayPointTypes.ThirdSector.GetNextSeparationPointType();

            Assert.Equal(WayPointTypes.Start3K,result);
        }

        [Fact]
        public void StartIsNextSeparationPointForFourthSector()
        {
            var result = WayPointTypes.FourthSector.GetNextSeparationPointType();

            Assert.Equal(WayPointTypes.Start,result);
        }
    }
}
