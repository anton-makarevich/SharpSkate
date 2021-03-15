using Sanet.SmartSkating.Dto.Models;

namespace Sanet.SmartSkating.Models.EventArgs
{
    public class WayPointEventArgs:System.EventArgs
    {
        public WayPointDto WayPoint { get; }
        
        public WayPointEventArgs(WayPointDto wayPoint)
        {
            WayPoint = wayPoint;
        }
    }
}