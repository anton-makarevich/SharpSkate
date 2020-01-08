using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Dto.Services;
using Sanet.SmartSkating.Models.EventArgs;
using Sanet.SmartSkating.Models.Location;
using Sanet.SmartSkating.Models.Training;

namespace Sanet.SmartSkating.Services.Location
{
    public abstract class BaseBleLocationService:IBleLocationService
    {
        protected readonly IBleDevicesProvider DevicesProvider;
        private List<BleDeviceDto> _devices;
        private const int RssiNearThreshold = -75;

        protected List<BleScansStack> ScanStacks { get; }

        protected BaseBleLocationService(IBleDevicesProvider devicesProvider)
        {
            DevicesProvider = devicesProvider;
            ScanStacks = new List<BleScansStack>();
        }

        public async Task LoadDevicesData()
        {
            _devices=await DevicesProvider.GetBleDevicesAsync();
            var t = 5;
        }

        protected  int GetWayPointForDeviceId(string deviceId)
        {
            if (_devices.Count == 0)
                return (int) WayPointTypes.Unknown;
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
            CheckIfCheckPointHasPassed();
        }

        private void CheckIfCheckPointHasPassed()
        {
            if (ScanStacks.Count<0)
                return;
            var closestStack = ScanStacks.OrderBy(f => f.AverageRssi).Last();

            if (!(closestStack.HasRssiTrendChanged && closestStack.RssiTrend == RssiTrends.Decrease))
                return;

            var closestWayPointType = (WayPointTypes)GetWayPointForDeviceId(closestStack.DeviceId);
            if (ScanStacks.Count == 1 && closestStack.AverageRssi > RssiNearThreshold)
            {
                InvokeCheckPointPassed(closestWayPointType,closestStack.Time);
                return;
            }

            var prevWayPointType = (int)closestWayPointType.GetPreviousSeparationPointType();
            var nextWayPointType = (int)closestWayPointType.GetNextSeparationPointType();

            BleScansStack prevStack;
            BleScansStack nextStack;
            if (ScanStacks.Count == 2)
            {
                prevStack = ScanStacks
                    .FirstOrDefault(s=>GetWayPointForDeviceId(s.DeviceId)==prevWayPointType);
                if (prevStack != null && prevStack.RssiTrend == RssiTrends.Decrease)
                {
                    InvokeCheckPointPassed(closestWayPointType,closestStack.Time);
                    return;
                }
                nextStack = ScanStacks
                    .FirstOrDefault(s=>GetWayPointForDeviceId(s.DeviceId)==nextWayPointType);
                if (nextStack != null && nextStack.RssiTrend == RssiTrends.Increase)
                {
                    InvokeCheckPointPassed(closestWayPointType,closestStack.Time);
                    return;
                }
            }

            prevStack = ScanStacks
                .FirstOrDefault(s=>GetWayPointForDeviceId(s.DeviceId)==prevWayPointType);
            nextStack = ScanStacks
                .FirstOrDefault(s=>GetWayPointForDeviceId(s.DeviceId)==nextWayPointType);
            if (nextStack != null 
                && nextStack.RssiTrend == RssiTrends.Increase 
                && prevStack != null 
                && prevStack.RssiTrend == RssiTrends.Decrease)
            {
                InvokeCheckPointPassed(closestWayPointType,closestStack.Time);
            }
        }
    }
}