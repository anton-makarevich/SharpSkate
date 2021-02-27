using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Dto.Services;
using Sanet.SmartSkating.Models.EventArgs;
using Sanet.SmartSkating.Models.Location;
using Sanet.SmartSkating.Services.Account;

namespace Sanet.SmartSkating.Services.Location
{
    public abstract class BaseBleLocationService:IBleLocationService
    {
        protected readonly IBleDevicesProvider DevicesProvider;
        private readonly IDataService _dataService;
        private readonly IAccountService _accountService;

        private List<BleDeviceDto>? _devices;
        private string _sessionId = string.Empty;
        private const int RssiNearThreshold = -75;

        public List<BleDeviceDto>? KnownDevices => _devices;

        protected List<BleScansStack> ScanStacks { get; }

        protected BaseBleLocationService(IBleDevicesProvider devicesProvider, IDataService dataService,
            IAccountService accountService)
        {
            DevicesProvider = devicesProvider;
            _dataService = dataService;
            _accountService = accountService;
            ScanStacks = new List<BleScansStack>();
        }

        public async Task LoadDevicesDataAsync()
        {
            _devices=await DevicesProvider.GetBleDevicesAsync();
        }
        protected  int GetWayPointForDeviceId(string deviceId)
        {
            if (_devices==null || _devices.Count == 0)
                return (int) WayPointTypes.Unknown;
            var deviceById = _devices.FirstOrDefault(f => f.Id == deviceId);
            return deviceById?.WayPointType ?? (int)WayPointTypes.Unknown;
        }

        public event EventHandler<CheckPointEventArgs>? CheckPointPassed;

        public virtual void StartBleScan(string sessionId)
        {
            _sessionId = sessionId;
            IsScanning = true;
        }

        public bool IsScanning { get; private set; }

        public virtual void StopBleScan()
        {
            IsScanning = false;
        }

        protected void InvokeCheckPointPassed(WayPointTypes type, DateTime time)
        {
            CheckPointPassed?.Invoke(this,new CheckPointEventArgs(type, time));
        }

        protected void ProceedNewScan(BleScanResultDto scan)
        {
            scan.ReceiverId = _accountService.DeviceId;
            var stack = ScanStacks.FirstOrDefault(f => f.DeviceId == scan.DeviceAddress);
            if (stack == null)
            {
                if (ScanStacks.Count == 4)
                    return;
                stack = new BleScansStack(scan.DeviceAddress);
                ScanStacks.Add(stack);
            }

            scan.SessionId = _sessionId;
            stack.AddScan(scan);
            _dataService.SaveBleScanAsync(scan);
            CheckIfCheckPointHasPassed();
        }

        private void CheckIfCheckPointHasPassed()
        {
            var closestStack = ScanStacks.OrderBy(f => f.AverageRssi).Last();

            if (!(closestStack.HasRssiTrendChanged && closestStack.RssiTrend == RssiTrends.Decrease))
                return;

            var closestWayPointType = (WayPointTypes)GetWayPointForDeviceId(closestStack.DeviceId);
            if (closestWayPointType == WayPointTypes.Unknown)
                return;
            if (ScanStacks.Count < 3)
            {
                if (closestStack.AverageRssi > RssiNearThreshold)
                  InvokeCheckPointPassed(closestWayPointType, closestStack.Time);
                return;
            }

            var rssiDifferences = ScanStacks
                .Select(s => Math.Abs(closestStack.AverageRssi - s.AverageRssi))
                .OrderBy(a=>a)
                .Skip(1)  // remove 0 (difference with itself)
                .ToList();

            if (rssiDifferences.Count > 1
                && rssiDifferences.First() > 10
                && rssiDifferences.Last() / rssiDifferences.First() < 2)
            {
                InvokeCheckPointPassed(closestWayPointType, closestStack.Time);
            }
        }
    }
}
