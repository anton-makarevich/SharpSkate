using System.Threading.Tasks;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Dto.Services;
using Sanet.SmartSkating.Services.Location;

namespace Sanet.SmartSkating.Xf.Droid.DummyServices.Services
{
    public class DummyBleLocationService:BaseBleLocationService
    {
        private readonly double _speedCoeff;
        private readonly IResourceReader _resourceReader;

        public DummyBleLocationService(IResourceReader resourceReader,
            IBleDevicesProvider devicesProvider,
            IDataService dataService,
            double speedCoeff = 1) 
            : base(devicesProvider,dataService)
        {
            _speedCoeff = speedCoeff;
            _resourceReader = resourceReader;
        }

        public override void StartBleScan(string sessionId)
        {
            base.StartBleScan(sessionId);
            
#pragma warning disable 4014
            RunDummyScan();
#pragma warning restore 4014
        }

        private async Task RunDummyScan()
        {
            var scans =
                await _resourceReader.ReadEmbeddedResourceAsync<DummyBleLocationService,BleScanResultDto>("Test.ble");
            for (var index = 0; index < scans.Count; index++)
            {
                if (!IsScanning)
                    return;
                var scan = scans[index];
                ProceedNewScan(scan);
                var nextScan = scans[index + 1];
                var timeToWait = nextScan.Time.Subtract(scan.Time).TotalMilliseconds;
                await Task.Delay((int)(timeToWait * _speedCoeff));
            }

            await RunDummyScan();
        }
    }
}