using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Dto.Services;
using Sanet.SmartSkating.Services.Location;
using Sanet.SmartSkating.Tests.Models.Location;
using Xunit;

namespace Sanet.SmartSkating.Tests.Services.Location
{
    public class BaseBleLocationServiceTests:BaseBleLocationService
    {
        private const string StartDeviceId = "start";
        private const string FinishDeviceId = "finish";
        private const string Start300MDeviceId = "start300M";
        private const string Start3KDeviceId = "start3K";
        
        private readonly BleDeviceDto _startDevice = new BleDeviceDto
        {
            Id = StartDeviceId,
            DeviceName = "test",
            ParentId = "me",
            WayPointType = 1
        };
        private readonly BleDeviceDto _finishDevice = new BleDeviceDto
        {
            Id = FinishDeviceId,
            DeviceName = "test",
            ParentId = "me",
            WayPointType = 3
        };
        private readonly BleDeviceDto _start300MDevice = new BleDeviceDto
        {
            Id = Start300MDeviceId,
            DeviceName = "test",
            ParentId = "me",
            WayPointType = 4
        };
        private readonly BleDeviceDto _start3KDevice = new BleDeviceDto
        {
            Id = Start3KDeviceId,
            DeviceName = "test",
            ParentId = "me",
            WayPointType = 6
        };

        private static readonly IBleDevicesProvider BleDevicesProvider = Substitute.For<IBleDevicesProvider>();

        public BaseBleLocationServiceTests() : this(BleDevicesProvider)
        {
        }
        
        protected BaseBleLocationServiceTests(IBleDevicesProvider devicesProvider) : base(devicesProvider)
        {
            devicesProvider.GetBleDevicesAsync().Returns(Task.FromResult(new List<BleDeviceDto>
            {
                _startDevice,
                _finishDevice,
                _start300MDevice,
                _start3KDevice
            }));
        }

        [Fact]
        public async Task FetchesDevices_OnServiceCreation()
        {
            await LoadDevicesDataAsync();
            await DevicesProvider.Received().GetBleDevicesAsync();
        }
        
        [Fact]
        public async Task GetWayPointForDeviceId_ReturnsUnknownId_WhenNoDevicesAreLoaded()
        {
            BleDevicesProvider.GetBleDevicesAsync().Returns(Task.FromResult(new List<BleDeviceDto>())); 
            await LoadDevicesDataAsync();
            var wayPointTypeInt = GetWayPointForDeviceId(StartDeviceId);

            wayPointTypeInt.Should().Be((int) WayPointTypes.Unknown);
        }

        [Fact]
        public async Task ReturnsCorrectWayPoint_ForStartId()
        {
            await LoadDevicesDataAsync();
            var wayPointTypeInt = GetWayPointForDeviceId(StartDeviceId);

            wayPointTypeInt.Should().Be((int) WayPointTypes.Start);
        }
        
        [Fact]
        public async Task ReturnsCorrectWayPoint_ForFinishId()
        {
            await LoadDevicesDataAsync();
            var wayPointTypeInt = GetWayPointForDeviceId(FinishDeviceId);

            wayPointTypeInt.Should().Be((int) WayPointTypes.Finish);
        }
        
        [Fact]
        public async Task ReturnsCorrectWayPoint_ForStart300MId()
        {
            await LoadDevicesDataAsync();
            var wayPointTypeInt = GetWayPointForDeviceId(Start300MDeviceId);

            wayPointTypeInt.Should().Be((int) WayPointTypes.Start300M);
        }
        
        [Fact]
        public async Task ReturnsCorrectWayPoint_ForStart3KId()
        {
            await LoadDevicesDataAsync();
            var wayPointTypeInt = GetWayPointForDeviceId(Start3KDeviceId);

            wayPointTypeInt.Should().Be((int) WayPointTypes.Start3K);
        }

        [Fact]
        public void CreatesNewStack_ForScansWithDifferentDeviceId()
        {
            const string deviceId = "device!";
            var scan = BleScansStackTests.GetScanDto(-3, DateTime.Now, deviceId);

            ProceedNewScan(scan);

            ScanStacks.Count.Should().Be(1);
            ScanStacks.First().DeviceId.Should().Be(deviceId);
        }

        [Fact]
        public void AddsUpTo4Stacks_ForDifferentDeviceIds()
        {
            for (var i = 1; i < 5; i++)
            {
                var deviceId = $"device{i}";
                var scan = BleScansStackTests.GetScanDto(-3, DateTime.Now, deviceId);

                ProceedNewScan(scan);
            }
            
            ScanStacks.Count.Should().Be(4);
        }
        
        [Fact]
        public void DoesNotAddMoreThan4Stacks_ForDifferentDeviceIds()
        {
            for (var i = 1; i < 7; i++)
            {
                var deviceId = $"device{i}";
                var scan = BleScansStackTests.GetScanDto(-3, DateTime.Now, deviceId);

                ProceedNewScan(scan);
            }
            
            ScanStacks.Count.Should().Be(4);
            ScanStacks.Last().DeviceId.Should().EndWith("4");
        }

        [Fact]
        public async Task
            FiresCheckPointPassed_WhenStackHasMaxAverage_AndAverageDifferenceToTheClosestIsAtLeastTwoTimeLessThanToTheFurthest()
        {
            await LoadDevicesDataAsync();

            AddTwoRandomScans(90, StartDeviceId);
            AddTwoRandomScans(80, Start300MDeviceId);
            AddTwoRandomScans(95, Start3KDeviceId);
            
            var checkPointType = WayPointTypes.Unknown;
            var checkPointPassedCalledTimes = 0;
            CheckPointPassed += (sender, args) =>
            {
                checkPointPassedCalledTimes++;
                checkPointType = args.WayPointType;
            };
            //Finish is maximum, changed to decreasing and should be reported
            AddCheckPointPassedScansAtFinishPoint();

            checkPointType.Should().Be(WayPointTypes.Finish);
            checkPointPassedCalledTimes.Should().Be(1);
        }
        
        [Fact]
        public async Task
            DoesNotFireCheckPointPassed_WhenStackHasMaxAverage_AndAverageDifferenceToTheClosestIsLessThanTwoTimeLessThanToTheFurthest()
        {
            await LoadDevicesDataAsync();

            AddTwoRandomScans(90, StartDeviceId);
            AddTwoRandomScans(60, Start300MDeviceId);
            AddTwoRandomScans(95, Start3KDeviceId);
            
            var checkPointType = WayPointTypes.Unknown;
            var checkPointPassedCalledTimes = 0;
            CheckPointPassed += (sender, args) =>
            {
                checkPointPassedCalledTimes++;
                checkPointType = args.WayPointType;
            };
            //Finish is maximum, changed to decreasing and should be reported
            AddCheckPointPassedScansAtFinishPoint();

            checkPointType.Should().Be(WayPointTypes.Unknown);
            checkPointPassedCalledTimes.Should().Be(0);
        }

        private void AddTwoRandomScans(int aboutRssi, string deviceId)
        {
            foreach (var unused in new[] {1, 2})
            {
                var rssi = -1 * new Random().Next(aboutRssi - 5, aboutRssi + 5);
                var scan = BleScansStackTests.GetScanDto(rssi, DateTime.Now, deviceId);

                ProceedNewScan(scan);
            }
        }
        
        private void AddTwoIncreasingScansForStart300MPoint()
        {
            foreach (var i in new[] {1, 2})
            {
                var scan = BleScansStackTests.GetScanDto(-80 + (10 * i), DateTime.Now, Start300MDeviceId);

                ProceedNewScan(scan);
            }
        }

        [Fact]
        public async Task FiresCheckPointPassed_WhenOnlySingleStackIsAvailableWithMaxAverage_AndChangedTrendToDecrease()
        {
            await LoadDevicesDataAsync();

            var checkPointType = WayPointTypes.Unknown;
            var checkPointPassedCalledTimes = 0;
            CheckPointPassed += (sender, args) =>
            {
                checkPointPassedCalledTimes++;
                checkPointType = args.WayPointType;
            };

            AddCheckPointPassedScansAtFinishPoint();

            checkPointType.Should().Be(WayPointTypes.Finish);
            checkPointPassedCalledTimes.Should().Be(1);
        }

        [Fact]
        public async Task FiresCheckPointPassed_WhenCorrespondingStackHasMaxAverage_ChangedTrendToDecrease_AndNextIncreasing()
        {
            await LoadDevicesDataAsync();
            
            // Start300M is increasing
            AddTwoIncreasingScansForStart300MPoint();

            var checkPointType = WayPointTypes.Unknown;
            var checkPointPassedCalledTimes = 0;
            CheckPointPassed += (sender, args) =>
            {
                checkPointPassedCalledTimes++;
                checkPointType = args.WayPointType;
            };
            //Finish is maximum, changed to decreasing and should be reported
            AddCheckPointPassedScansAtFinishPoint();

            checkPointType.Should().Be(WayPointTypes.Finish);
            checkPointPassedCalledTimes.Should().Be(1);
        }

        [Fact]
        public void SetsIsScanningToTrue_WhenStartIsCalled()
        {
            base.StartBleScan();
            IsScanning.Should().BeTrue();
        }
        
        [Fact]
        public void SetsIsScanningToFalse_WhenStopIsCalled()
        {
            base.StopBleScan();
            IsScanning.Should().BeFalse();
        }

        private void AddCheckPointPassedScansAtFinishPoint()
        {
            foreach (var rssi in new[] {-50, -50, -60})
            {
                var scan = BleScansStackTests.GetScanDto(rssi, DateTime.Now, FinishDeviceId);

                ProceedNewScan(scan);
            }
        }

        public override void StartBleScan()
        {
            throw new NotImplementedException();
        }

        public override void StopBleScan()
        {
            throw new NotImplementedException();
        }
    }
}