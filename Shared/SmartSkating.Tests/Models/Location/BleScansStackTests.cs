using System;
using FluentAssertions;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Models.Location;
using Xunit;

namespace Sanet.SmartSkating.Tests.Models.Location
{
    public class BleScansStackTests
    {
        private readonly BleScansStack _sut = new BleScansStack("deviceId");
        
        [Fact]
        public void AcceptsId_WhenCreated()
        {
            const string deviceId = "someDevice";
            
            var sut = new BleScansStack(deviceId);

            sut.DeviceId.Should().Be(deviceId);
        }

        [Fact]
        public void AverageRssiIsEqual_ToRssiOfSingleValue()
        {
            var scan = GetScanDto(-45, DateTime.Now);
     
            _sut.AddScan(scan);

            _sut.AverageRssi.Should().Be(scan.Rssi);
        }

        [Fact]
        public void AverageRssiIsEqual_ToAverageValueOfTwoScans()
        {
            var scan1 = GetScanDto(-20, DateTime.Now);
            var scan2 = GetScanDto(-60, DateTime.Now);
            
            _sut.AddScan(scan1);
            _sut.AddScan(scan2);

            _sut.AverageRssi.Should().Be((scan1.Rssi + scan2.Rssi) / 2);
        }
        
        [Fact]
        public void TakesTwoLastScans_WhenCalculatesAverage()
        {
            var scan0 = GetScanDto(-10, DateTime.Now);
            var scan1 = GetScanDto(-20, DateTime.Now);
            var scan2 = GetScanDto(-60, DateTime.Now);
            
            _sut.AddScan(scan0);
            _sut.AddScan(scan1);
            _sut.AddScan(scan2);

            _sut.AverageRssi.Should().Be((scan1.Rssi + scan2.Rssi) / 2);
        }

        [Fact]
        public void DefaultAverageValue_IsMinusOneHundred()
        {
            _sut.AverageRssi.Should().Be(-100);
        }

        [Fact]
        public void AverageDoesNotChange_WhenAddingScanWithWrongId()
        {
            var scan = GetScanDto(-45, DateTime.Now, "wrongId");
     
            _sut.AddScan(scan);

            _sut.AverageRssi.Should().Be(-100);
        }

        [Fact]
        public void InitialTrendValue_IsSame()
        {
            _sut.RssiTrend.Should().Be(RssiTrends.Same);
        }

        [Fact]
        public void TrendIncreasing_WhenNewAverageIsHigherThanPrevious()
        {
            var scan1 = GetScanDto(-80, DateTime.Now);
            var scan2 = GetScanDto(-60, DateTime.Now);
            
            _sut.AddScan(scan1);
            _sut.AddScan(scan2);

            _sut.RssiTrend.Should().Be(RssiTrends.Increase);
        }
        
        [Fact]
        public void TrendDecreasing_WhenNewAverageIsLowerThanPrevious()
        {
            var scan0 = GetScanDto(-60, DateTime.Now);
            var scan1 = GetScanDto(-60, DateTime.Now);
            var scan2 = GetScanDto(-100, DateTime.Now);
            
            _sut.AddScan(scan0);
            _sut.AddScan(scan1);
            _sut.AddScan(scan2);

            _sut.RssiTrend.Should().Be(RssiTrends.Decrease);
        }

        [Fact]
        public void InitialValuesForHasRssiTrendChanged_IsFalse()
        {
            _sut.HasRssiTrendChanged.Should().Be(false);
        }

        [Fact]
        public void InitialTime_HasMinValue()
        {
            _sut.Time.Should().Be(DateTime.MinValue);
        }
        
        [Fact]
        public void UpdatesTimeWithTomeOfScan_WhenScanAdded()
        {
            var time = DateTime.Now;

            var scan = GetScanDto(-1, time);
            _sut.AddScan(scan);
            
            _sut.Time.Should().Be(time);
        }

        [Fact]
        public void HasRssiTrendChangedIsFalse_WhenAddScanDoesNotChangeTrendDirection()
        {
            var scan1 = GetScanDto(-80, DateTime.Now);
            var scan2 = GetScanDto(-60, DateTime.Now);
            
            _sut.AddScan(scan1);
            _sut.AddScan(scan2);

            _sut.HasRssiTrendChanged.Should().Be(false);
        }

        [Fact]
        public void HasRssiTrendChangedIsTrue_WhenAddScanChangesTrendDirection()
        {
            var scan0 = GetScanDto(-60, DateTime.Now);
            var scan1 = GetScanDto(-60, DateTime.Now);
            var scan2 = GetScanDto(-100, DateTime.Now);
            
            _sut.AddScan(scan0);
            _sut.AddScan(scan1);
            _sut.AddScan(scan2);

            _sut.HasRssiTrendChanged.Should().Be(true);
        }

        public static BleScanResultDto GetScanDto(int rssi, DateTime time, string deviceId = "deviceId")
        {
            return new BleScanResultDto
            {
                DeviceAddress = deviceId,
                Rssi = rssi,
                Time = time
            };
        }
    }
}