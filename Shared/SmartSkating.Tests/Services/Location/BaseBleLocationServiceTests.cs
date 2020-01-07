using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Dto.Services;
using Sanet.SmartSkating.Services.Location;
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

        public BaseBleLocationServiceTests() : this(Substitute.For<IBleDevicesProvider>())
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
        public async Task FetchesDevices_WhenTheListIsEmpty()
        {
            await GetWayPointForDeviceId(StartDeviceId);

            await DevicesProvider.Received().GetBleDevicesAsync();
        }

        [Fact]
        public async Task ReturnsCorrectWayPoint_ForStartId()
        {
            var wayPointTypeInt = await GetWayPointForDeviceId(StartDeviceId);

            wayPointTypeInt.Should().Be((int) WayPointTypes.Start);
        }
        
        [Fact]
        public async Task ReturnsCorrectWayPoint_ForFinishId()
        {
            var wayPointTypeInt = await GetWayPointForDeviceId(FinishDeviceId);

            wayPointTypeInt.Should().Be((int) WayPointTypes.Finish);
        }
        
        [Fact]
        public async Task ReturnsCorrectWayPoint_ForStart300MId()
        {
            var wayPointTypeInt = await GetWayPointForDeviceId(Start300MDeviceId);

            wayPointTypeInt.Should().Be((int) WayPointTypes.Start300M);
        }
        
        [Fact]
        public async Task ReturnsCorrectWayPoint_ForStart3KId()
        {
            var wayPointTypeInt = await GetWayPointForDeviceId(Start3KDeviceId);

            wayPointTypeInt.Should().Be((int) WayPointTypes.Start3K);
        }

        public override void StartBleScan()
        {
            throw new System.NotImplementedException();
        }

        public override void StopBleScan()
        {
            throw new System.NotImplementedException();
        }
    }
}