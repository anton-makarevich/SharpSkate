using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using BleWriter.Services;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Models;
using Sanet.SmartSkating.Models.EventArgs;
using Sanet.SmartSkating.Services.Location;
using Sanet.SmartSkating.ViewModels.Base;

namespace BleWriter.ViewModels
{
    public class BleWriterViewModel:BaseViewModel
    {
        private readonly IBleLocationService _bleLocationService;
        private readonly IBleWriterService _bleWriterService;

        public ObservableCollection<BleDeviceDto> BleDevices { get; } = new ObservableCollection<BleDeviceDto>();

        public BleWriterViewModel(IBleLocationService bleLocationService, IBleWriterService bleWriterService)
        {
            _bleLocationService = bleLocationService;
            _bleWriterService = bleWriterService;
        }

        public ICommand StopScanCommand => new SimpleCommand(StopBleScan);
        public ICommand WriteIdsCommand => new SimpleCommand(async() =>
        {
            await WriteDeviceIds();
        });

        private async Task WriteDeviceIds()
        {
            foreach (var bleDevice in BleDevices)
            {
                await _bleWriterService.WriteDeviceIdAsync(bleDevice);    
            }
        }

        private void StopBleScan()
        {
            _bleLocationService.StopBleScan();
            _bleLocationService.NewBleDeviceFound += BleLocationServiceOnNewBleDeviceFound;
        }

        public override void AttachHandlers()
        {
            base.AttachHandlers();
            _bleLocationService.NewBleDeviceFound += BleLocationServiceOnNewBleDeviceFound;
            _bleLocationService.StartBleScan();
        }

        private void BleLocationServiceOnNewBleDeviceFound(object sender, BleDeviceEventArgs e)
        {
            BleDevices.Add(e.BleDevice);
            if (BleDevices.Count == 4)
                StopBleScan();
        }
    }
}