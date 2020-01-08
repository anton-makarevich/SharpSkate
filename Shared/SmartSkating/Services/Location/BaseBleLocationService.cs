using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Dto.Services;
using Sanet.SmartSkating.Models.EventArgs;
using Sanet.SmartSkating.Models.Location;

namespace Sanet.SmartSkating.Services.Location
{
    public abstract class BaseBleLocationService:IBleLocationService
    {
        protected readonly IBleDevicesProvider DevicesProvider;
        private readonly List<BleDeviceDto> _devices = new List<BleDeviceDto>();
        
        protected List<BleScansStack> ScanStacks { get; }

        protected BaseBleLocationService(IBleDevicesProvider devicesProvider)
        {
            DevicesProvider = devicesProvider;
            ScanStacks = new List<BleScansStack>();
        }

        protected async Task<int> GetWayPointForDeviceId(string deviceId)
        {
            if (_devices.Count == 0)
                _devices.AddRange(await DevicesProvider.GetBleDevicesAsync());
            var deviceById = _devices.FirstOrDefault(f => f.Id == deviceId);
            return deviceById?.WayPointType ?? (int)WayPointTypes.Unknown;
        }

        public event EventHandler<CheckPointEventArgs>? CheckPointPassed;
        
        public abstract void StartBleScan();
        
        public abstract void StopBleScan();

        protected void InvokeCheckPointPassed(WayPointTypes type, DateTime time)
        {
            CheckPointPassed?.Invoke(this,new CheckPointEventArgs(type, time));
        }
        
        protected void ProceedNewScan(BleScanResultDto scan)
        {
            var stack = ScanStacks.FirstOrDefault(f => f.DeviceId == scan.DeviceAddress);
            if (stack == null)
            {
                if (ScanStacks.Count == 4)
                    return;
                stack = new BleScansStack(scan.DeviceAddress);
                ScanStacks.Add(stack);
            }
            stack.AddScan(scan);
        }
    }
}