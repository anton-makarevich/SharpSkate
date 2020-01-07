using System.Collections.Generic;
using System.Linq;
using Sanet.SmartSkating.Dto.Models;

namespace Sanet.SmartSkating.Models.Location
{
    public class BleScansStack
    {
        private const int MinimumRssiValue = -100;
        public string DeviceId { get; }
        public int AverageRssi { get; private set; } = MinimumRssiValue;
        public RssiTrends RssiTrend { get; private set; }

        private readonly Queue<BleScanResultDto> _stack = new Queue<BleScanResultDto>();

        public BleScansStack(string deviceId)
        {
            DeviceId = deviceId;
        }

        public void AddScan(BleScanResultDto scan)
        {
            if (scan.DeviceAddress!=DeviceId)
                return;

            if (_stack.Count == 2)
                _stack.Dequeue();
            
            _stack.Enqueue(scan);
            var prevAverage = AverageRssi;
            AverageRssi = (int)_stack.Average(f => f.Rssi);
            var trend = AverageRssi - prevAverage;
            if (trend > 0)
                RssiTrend = RssiTrends.Increase;
            else if (trend < 0)
                RssiTrend = RssiTrends.Decrease;
            else
                RssiTrend = RssiTrends.Same;
        }
    }
}