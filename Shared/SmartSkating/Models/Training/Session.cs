using System;
using System.Collections.Generic;
using System.Linq;
using Sanet.SmartSkating.Models.Geometry;
using Sanet.SmartSkating.Utils;

namespace Sanet.SmartSkating.Models.Training
{
    public class Session : ISession
    {
        private readonly Rink _rink;

        public Session(Rink rink)
        {
            _rink = rink;
            WayPoints = new List<WayPoint>();
            Sectors = new List<Section>();
        }

        public IList<WayPoint> WayPoints { get; }
        public IList<Section> Sectors { get; }
        public int LapsCount { get; private set; }
        public TimeSpan LastLapTime { get; private set; }
        public Rink Rink => _rink;

        public void AddPoint(Coordinate location, DateTime date)
        {
            var point = _rink.ToLocalCoordinateSystem(location);
            if (!((point, _rink.Center).GetDistance() <= 100)) return;

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
            
            if (type == WayPointTypes.Unknown)
                return;
            
            if (WayPoints.Any() && WayPoints.Last().Type != type)
            {
                var lastPoint = WayPoints.Last();
                var separatingPointType = (lastPoint.Type, type).GetTypeSeparatingSectors();
                if (separatingPointType == WayPointTypes.Unknown)
                    return;
                var separatingPointLocation = separatingPointType.GetSeparatingPointLocationForType(_rink);
                var distanceTo
                        = (lastPoint.AdjustedCoordinate, separatingPointLocation).GetRelativeDistance();
                    var distanceFrom
                        = (adjustedLocation, separatingPointLocation).GetRelativeDistance();
                    var separatingPointDate = InterpolateDate(
                        lastPoint.Date, 
                        date,
                        distanceTo,
                        distanceFrom);
                    AddSeparatingPoint(separatingPointLocation,separatingPointDate,separatingPointType);
            }

            WayPoints.Add(new WayPoint(
                location,
                adjustedLocation,
                date,
                type));
        }

        public void AddSeparatingPoint(Coordinate location, DateTime date, WayPointTypes type)
        {
            if (WayPoints.Any())
            {
                var previousSectorType = type.GetPreviousSectorType();
                if (WayPoints.Last().Type!=previousSectorType)
                    return;
            }

            var separatingWayPoint = new WayPoint(
                location,
                location,
                date,
                type);
            AddSection(separatingWayPoint);
            WayPoints.Add(separatingWayPoint);
        }

        private void AddSection(WayPoint separatingWayPoint)
        {
            var firstPointType = separatingWayPoint.Type.GetPreviousSeparationPointType();
            var firstPoint = WayPoints.LastOrDefault(wp => wp.Type == firstPointType);
            if (firstPoint.Type == WayPointTypes.Unknown)
            {
                firstPointType = separatingWayPoint.Type.GetPreviousSectorType();
                firstPoint = WayPoints.LastOrDefault(wp => wp.Type == firstPointType);
                if (firstPoint.Type == WayPointTypes.Unknown)
                    return;
            }
            var section = new Section(firstPoint,separatingWayPoint);
            Sectors.Add(section);

            UpdateMetaData();
        }

        private void UpdateMetaData()
        {
            var lastSections = Sectors.TakeLast(4).ToList();
            if (lastSections.Count() == 4 && lastSections.First().Type == WayPointTypes.FirstSector &&
                lastSections.Last().Type == WayPointTypes.FourthSector)
            {
                LapsCount++;
                LastLapTime = new TimeSpan(lastSections.Sum(s => s.Time.Ticks));
            }
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
    }
}