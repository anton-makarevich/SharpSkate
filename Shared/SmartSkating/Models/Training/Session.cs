using System;
using System.Collections.Generic;
using System.Linq;
using Sanet.SmartSkating.Models.Geometry;
using Sanet.SmartSkating.Utils;

namespace Sanet.SmartSkating.Models.Training
{
    public class Session
    {
        private readonly Rink _rink;

        public Session(Rink rink)
        {
            _rink = rink;
            WayPoints = new List<WayPoint>();
        }

        public IList<WayPoint> WayPoints { get; }

        public void Add(Coordinate location, DateTime? date = null)
        {
            var dateTime = date ?? DateTime.Now;
                date = DateTime.Now;
            var point = _rink.ToLocalCoordinateSystem(location);
            var type =
                (from sector in _rink.Sectors where sector.Contains(point)
                    select sector.Type).FirstOrDefault();
            var adjustedLocation = location;
            if (type == WayPointTypes.Unknown)
            {
                var closestSector = _rink.Sectors
                    .OrderBy(s => (s.Center, point).GetDistance())
                    .First();
                var intersection = closestSector
                    .FindIntersection(new Line(point, closestSector.Center));
                if (intersection.HasValue)
                {
                    adjustedLocation = _rink.ToGeoCoordinateSystem(intersection.Value);
                    type = closestSector.Type;
                }
            }
            
            if ((point,_rink.Center).GetDistance()<=100)
                WayPoints.Add(new WayPoint(
                    location,
                    adjustedLocation,
                    dateTime,
                    type));
        }
    }
}