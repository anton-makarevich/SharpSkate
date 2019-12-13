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
        private WayPoint? _lastWayPoint;

        public Session(Rink rink)
        {
            _rink = rink;
            WayPoints = new List<WayPoint>();
        }

        public IList<WayPoint> WayPoints { get; }
        public IList<Section> Sectors { get; }

        public void Add(Coordinate location, DateTime? date = null)
        {
            var point = _rink.ToLocalCoordinateSystem(location);
            if (!((point, _rink.Center).GetDistance() <= 100)) return;
            
            var dateTime = date ?? DateTime.Now;
            
            var type =
                (from sector in _rink.Sectors 
                    where sector.Contains(point)
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

            if (_lastWayPoint.HasValue && _lastWayPoint.Value.Type != type)
            {
                var lastPoint = _lastWayPoint ?? new WayPoint(location, dateTime);
                var extraPointType = (lastPoint.Type, type).GetTypeSeparatingSectors();
                if (extraPointType == WayPointTypes.Unknown)
                    return;
                var extraLocation = GetLocationByType(extraPointType);
                if (extraLocation.HasValue)
                {
                    var extraLocationValue = extraLocation.Value;
                    var prevDistance
                        = (lastPoint.AdjustedCoordinate, extraLocationValue).GetRelativeDistance();
                    var currentDistance
                        = (adjustedLocation, extraLocationValue).GetRelativeDistance();
                    var extraDate = InterpolateDate(
                        lastPoint.Date, 
                        dateTime,
                        prevDistance,
                        currentDistance);
                    WayPoints.Add(new WayPoint(
                        extraLocationValue,
                        extraLocationValue,
                        extraDate,
                        extraPointType));
                }
            }

            _lastWayPoint = new WayPoint(
                location,
                adjustedLocation,
                dateTime,
                type);
            WayPoints.Add(_lastWayPoint.Value);
        }

        private DateTime InterpolateDate(
            DateTime previousDate, 
            DateTime currentDate, 
            double previousDistance,
            double currentDistance)
        {
            var wholeTicks = currentDate.Ticks - previousDate.Ticks;
            var wholeDistance = previousDistance + currentDistance;

            var deltaTicks = wholeTicks * previousDistance / wholeDistance;

            return new DateTime(previousDate.Ticks+(long)deltaTicks);
        }

        private Coordinate? GetLocationByType(WayPointTypes pointType)
        {
            return pointType switch
            {
                WayPointTypes.Start => _rink.Start,
                WayPointTypes.Finish => _rink.Finish,
                WayPointTypes.Start300M => _rink.Start300M,
                WayPointTypes.Start3K => _rink.Start3K,
                _ => throw new NotSupportedException($"No coordinate for {pointType} point type")
            };
        }
    }
}