using CoreBluetooth;
using Sanet.SmartSkating.Dto.Services;
using Sanet.SmartSkating.Services.Location;

namespace Sanet.SmartSkating.Xf.Ios.Location
{
    public class IosBleService:BaseBleLocationService
    {
        private readonly CBCentralManager _centralManager = new CBCentralManager();
        public IosBleService(IBleDevicesProvider devicesProvider) : base(devicesProvider)
        {
        }

        public override void StartBleScan()
        {
            if (KnownDevices==null || KnownDevices.Count==0)
                return;
            base.StartBleScan();
            
            _centralManager.DiscoveredPeripheral+= CentralManagerOnDiscoveredPeripheral;
            _centralManager.ScanForPeripherals(new CBUUID[0]);
        }

        public override void StopBleScan()
        {
            base.StopBleScan();
            _centralManager.DiscoveredPeripheral -= CentralManagerOnDiscoveredPeripheral;
            _centralManager.StopScan();
        }

        private void CentralManagerOnDiscoveredPeripheral(object sender, CBDiscoveredPeripheralEventArgs e)
        {
            // TODO find a way to map beacons
        }
    }
}