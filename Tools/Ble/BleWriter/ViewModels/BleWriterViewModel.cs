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
        private bool _canWrite;
        private bool _canStopScanning;

        public ObservableCollection<BleDeviceDto> BleDevices { get; } = new ObservableCollection<BleDeviceDto>();

        public BleWriterViewModel(IBleLocationService bleLocationService, IBleWriterService bleWriterService)
        {
            _bleLocationService = bleLocationService;
            _bleWriterService = bleWriterService;
        }

        public ICommand StopScanCommand => new SimpleCommand(StopBleScan);
        public ICommand WriteIdsCommand => new SimpleCommand(async() =>
        {
            if (CanWrite)
                await WriteDeviceIds();
        });

        public bool CanWrite    
        {
            get => BleDevices.Count != 0 && _canWrite;
            private set => SetProperty(ref _canWrite, value);
        }

        public bool CanStopScanning   
        {
            get => _canStopScanning;
            private set => SetProperty(ref _canStopScanning, value);
        }

        private async Task WriteDeviceIds()
        {
            foreach (var bleDevice in BleDevices)
            {
                await _bleWriterService.WriteDeviceIdAsync(bleDevice);    
            }
        }

        private void StopBleScan()
        {
            if (!CanStopScanning)
                return;
            _bleLocationService.StopBleScan();
            CanWrite = true;
            CanStopScanning = false;
            _bleLocationService.NewBleDeviceFound += BleLocationServiceOnNewBleDeviceFound;
        }

        public override void AttachHandlers()
        {
            base.AttachHandlers();
            _bleLocationService.NewBleDeviceFound += BleLocationServiceOnNewBleDeviceFound;
            StartScanning();
        }

        private void StartScanning()
        {
            CanStopScanning = true;
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