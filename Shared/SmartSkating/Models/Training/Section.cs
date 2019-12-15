using System;

namespace Sanet.SmartSkating.Models.Training
{
    public struct Section
    {
        public Section(WayPoint startWayPoint, WayPoint finishWayPoint)
        {
            Time = finishWayPoint.Date.Subtract(startWayPoint.Date);
            Type = finishWayPoint.Type.GetPreviousSectorType();
        }

        public WayPointTypes Type { get; }
        
        public TimeSpan Time { get; }
    }
}