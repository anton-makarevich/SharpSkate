using System.Threading.Tasks;
using BleWriter.Services;
using BleWriter.ViewModels;
using FluentAssertions;
using NSubstitute;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Models.EventArgs;
using Sanet.SmartSkating.Services.Location;
using Xunit;

namespace BleWriter.Core.Tests.ViewModelTests
{
    public class BleWriterViewModelTests
    {
        private readonly BleWriterViewModel _sut;
        private readonly IBleLocationService _bleLocationService;
        private readonly IBleWriterService _bleWriterService;

        public BleWriterViewModelTests()
        {
            _bleLocationService = Substitute.For<IBleLocationService>();
            _bleWriterService = Substitute.For<IBleWriterService>();
            _sut = new BleWriterViewModel(_bleLocationService, _bleWriterService);
        }


        [Fact]
        public void StartScanForBleDevices_WhenPageIsLoaded()
        {
            _sut.AttachHandlers();
            
            _bleLocationService.Received().StartBleScan();
        }

        [Fact]
        public void StopsBleScan_WhenStopCommandIsExecuted()
        {
            _sut.StopScanCommand.Execute(null);
            
            _bleLocationService.Received().StopBleScan();
        }

        [Fact]
        public void StopsScan_WhenFourDevicesAreReceived()
        {
            _sut.AttachHandlers();
            
            for (var i = 0;i<4;i++)
                _bleLocationService.NewBleDeviceFound += Raise.EventWith(new BleDeviceEventArgs(new BleDeviceDto()));
            
            _bleLocationService.Received().StopBleScan();
        }

        [Fact]
        public void AddsBleDeviceToCollection_WhenReceivesItFromService()
        {
            _sut.AttachHandlers();
            
            _bleLocationService.NewBleDeviceFound += Raise.EventWith(new BleDeviceEventArgs(new BleDeviceDto()));

            _sut.BleDevices.Count.Should().Be(1);
        }

        [Fact]
        public async Task PassesEveryDeviceToWriterService_WhenWriteIdsCommandIsExecuted()
        {
            var deviceStub = new BleDeviceDto();
            _sut.AttachHandlers();
            _bleLocationService.NewBleDeviceFound += Raise.EventWith(new BleDeviceEventArgs(deviceStub));

            _sut.WriteIdsCommand.Execute(null);

            await _bleWriterService.Received().WriteDeviceIdAsync(deviceStub);
        }
    }
}