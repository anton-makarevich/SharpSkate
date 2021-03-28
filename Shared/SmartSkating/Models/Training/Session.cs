using System;
using System.Collections.Generic;
using System.Linq;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Models.Geometry;
using Sanet.SmartSkating.Models.Location;
using Sanet.SmartSkating.Services;
using Sanet.SmartSkating.Utils;

namespace Sanet.SmartSkating.Models.Training
{
    public class Session : ISession
    {
        private readonly Rink _rink;
        private readonly ISettingsService _settingsService;

        public Session(Rink rink, ISettingsService settingsService):
            this(Guid.NewGuid().ToString("N"),rink,settingsService, false,false)
        { }

        public Session(string sessionId, Rink rink, ISettingsService settingsService, bool isRemote, bool isCompleted)
        {
            _rink = rink;
            _settingsService = settingsService;
            WayPoints = new List<WayPoint>();
            Sectors = new List<Section>();
            SessionId = sessionId;
            IsRemote = isRemote;
            IsCompleted = isCompleted;
        }

        public string SessionId { get; }
        public IList<WayPoint> WayPoints { get; }
        public IList<Section> Sectors { get; }
        public int LapsCount { get; private set; }
        public TimeSpan LastLapTime { get; private set; }
        public TimeSpan BestLapTime { get; private set; }
        public Rink Rink => _rink;

        public void AddPoint(Coordinate location, DateTime date)
        {
            LastCoordinate = location;
            var point = _rink.ToLocalCoordinateSystem(location);
            var adjustedPoint = point;
            if (!((point, _rink.Center).GetDistance() <= 100)) return;

            // we only record sectors in normal CCW order
            var expectedSectorTypes = WayPoints.Count>0
                ? new List<WayPointTypes>
                {
                    WayPoints.Last().Type,
                    WayPoints.Last().Type.GetNextSectorType()
                }
                : new List<WayPointTypes>();

            if (expectedSectorTypes.Count > 1 && _settingsService.CanInterpolateSectors)
            {
                expectedSectorTypes.Add(WayPoints.Last().Type.GetNextSectorType().GetNextSectorType());
            }

            var expectedSectors = expectedSectorTypes.Count>0
                ? _rink.Sectors.Where(s => expectedSectorTypes.Contains(s.Type)).ToList()
                : _rink.Sectors;

            var type =
                (from sector in _rink.Sectors
                    where sector.Contains(point)
                    select sector.Type).FirstOrDefault();
            var adjustedLocation = location;
            if (type == WayPointTypes.Unknown)
            {
                var closestSector = expectedSectors
                    .OrderBy(s => (s.Center, point).GetDistance())
                    .First();
                var intersection = closestSector
                    .FindIntersection(new Line(point, closestSector.Center));
                if (intersection.HasValue)
                {
                    adjustedLocation = _rink.ToGeoCoordinateSystem(intersection.Value);
                    adjustedPoint = intersection.Value;
                    type = closestSector.Type;
                }
            }

            if (_settingsService.CanInterpolateSectors)
            {
                AddMissingSector(date, expectedSectorTypes, type, adjustedPoint);
            }

            if (type == WayPointTypes.Unknown)
                return;

            if (WayPoints.Count>0 && WayPoints.Last().Type != type)
            {
                var lastPoint = WayPoints.Last();
                var separatingPointType = (lastPoint.Type, type).GetTypeSeparatingSectors();
                if (separatingPointType == WayPointTypes.Unknown)
                    return;
                var separatingPointLocation = separatingPointType.GetSeparatingPointLocationForType(_rink);
                var distanceTo
                        = (_rink.ToLocalCoordinateSystem(lastPoint.AdjustedCoordinate), separatingPointLocation.Item1).GetDistance();
                var distanceFrom
                    = (adjustedPoint, separatingPointLocation.Item1).GetDistance();
                var separatingPointDate = InterpolateDate(
                    lastPoint.Date,
                    date,
                    distanceTo,
                    distanceFrom);
                AddSeparatingPoint(separatingPointType, separatingPointDate);
            }

            WayPoints.Add(new WayPoint(
                location,
                adjustedLocation,
                date,
                type));
        }

        public void AddPoints(IEnumerable<WayPointDto> waypoints)
        {
            WayPoints.Clear();
            foreach (var waypoint in waypoints.OrderBy(w => w.Time))
            {
                AddPoint(new Coordinate(waypoint.Coordinate), waypoint.Time);
            }
        }

        private void AddMissingSector(DateTime date,
            IReadOnlyList<WayPointTypes> expectedSectorTypes,
            WayPointTypes type,
            Point adjustedPoint)
        {
            if (expectedSectorTypes.Count <= 0
                || type != expectedSectorTypes.Last()
                || !(date.Subtract(WayPoints.Last().Date).TotalSeconds > MaxSectorTimeInSeconds)) return;
            var lastPoint = WayPoints.Last();
            var missingSector = _rink.Sectors
                .Single(s => s.Type == expectedSectorTypes[1]);
            var distanceTo
                = (_rink.ToLocalCoordinateSystem(lastPoint.AdjustedCoordinate), missingSector.Center).GetDistance();
            var distanceFrom
                = (missingSector.Center, adjustedPoint).GetDistance();
            var missingSectorDate = InterpolateDate(
                lastPoint.Date,
                date,
                distanceTo,
                distanceFrom);
            AddPoint(_rink.ToGeoCoordinateSystem(missingSector.Center), missingSectorDate);
        }

        public void AddSeparatingPoint(WayPointTypes type, DateTime date)
        {
            var location = type.GetSeparatingPointLocationForType(_rink).Item2;
            if (WayPoints.Count>0)
            {
                var previousSectorType = type.GetPreviousSectorType();
                var previousSeparationType = type.GetPreviousSeparationPointType();
                var lastWayPoint = WayPoints.Last();
                if (lastWayPoint.Type!=previousSectorType && lastWayPoint.Type!=previousSeparationType)
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

        public DateTime StartTime { get; private set; }
        public Section? BestSector { get; private set; }

        public void SetStartTime(DateTime startTime)
        {
            StartTime = startTime;
        }

        public Coordinate? LastCoordinate { get; private set; }
        public bool IsRemote { get; }
        public bool IsCompleted { get; }

        private void AddSection(WayPoint separatingWayPoint)
        {
            var firstPointType = separatingWayPoint.Type.GetPreviousSeparationPointType();
            var firstPoint = WayPoints.LastOrDefault(wp => wp.Type == firstPointType);
            if (firstPoint.Type == WayPointTypes.Unknown)
            {
                firstPointType = separatingWayPoint.Type.GetPreviousSectorType();
                firstPoint = GetFirstPointOfCurrentSector(firstPointType);
                if (firstPoint.Type == WayPointTypes.Unknown)
                    return;
            }
            var section = new Section(firstPoint,separatingWayPoint);

            if (Sectors.Count>0 )
                if (BestSector == null || BestSector.Value.Time.TotalMilliseconds > section.Time.TotalMilliseconds)
                    BestSector = section;

            Sectors.Add(section);

            UpdateMetaData();
        }

        private WayPoint GetFirstPointOfCurrentSector(WayPointTypes sectorType)
        {
            if (WayPoints.Count == 1 && WayPoints[0].Type == sectorType)
                return WayPoints[0];
            for (var index = WayPoints.Count - 1; index >= 0; index--)
            {
                if (WayPoints[index].Type != sectorType && WayPoints[index + 1].Type == sectorType)
                    return WayPoints[index + 1];
                if (index == 0 && WayPoints[index].Type == sectorType)
                    return WayPoints[index];
            }
            return default;
        }

        private void UpdateMetaData()
        {
            var lastSections = Sectors.TakeLast(4).ToList();
            if (lastSections.Count() == 4 && lastSections.First().Type == WayPointTypes.FirstSector &&
                lastSections.Last().Type == WayPointTypes.FourthSector)
            {
                LapsCount++;
                LastLapTime = new TimeSpan(lastSections.Sum(s => s.Time.Ticks));
                if (BestLapTime.Ticks == 0 || LastLapTime.Ticks < BestLapTime.Ticks)
                    BestLapTime = LastLapTime;
            }
        }

        private DateTime InterpolateDate(
            DateTime previousDate,
            DateTime currentDate,
            double previousDistance,
            double currentDistance)
        {
            var wholeTicks = currentDate.Subtract(previousDate).Ticks;
            var wholeDistance = previousDistance + currentDistance;

            var deltaTicks = wholeTicks * previousDistance / wholeDistance;

            return new DateTime(previousDate.Ticks+(long)deltaTicks);
        }

        private double MaxSectorTimeInSeconds
        {
            get
            {
                if (BestLapTime.Ticks == 0)
                    return 25;
                return BestLapTime.TotalSeconds * 0.39;
            }
        }
    }
}
