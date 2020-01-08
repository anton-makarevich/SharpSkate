using System;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Models.Training;

namespace Sanet.SmartSkating.Models.EventArgs
{
    public class CheckPointEventArgs:System.EventArgs
    {
        public WayPointTypes WayPointType { get; }
        public DateTime? Date { get; }

        public CheckPointEventArgs(WayPointTypes wayPointType, DateTime? date = null)
        {
            WayPointType = wayPointType;
            Date = date;
        }
    }
}