using System;

namespace Sanet.SmartSkating.Models.Training
{
    public struct Section
    {
        public Section(WayPoint startWayPoint, WayPoint finishWayPoint)
        {
            Time = finishWayPoint.Date.Subtract(startWayPoint.Date);
            Type = (startWayPoint.Type, finishWayPoint.Type).GetSectorTypeBetween();
        }

        public WayPointTypes Type { get; }
        
        public TimeSpan Time { get; }
    }
}