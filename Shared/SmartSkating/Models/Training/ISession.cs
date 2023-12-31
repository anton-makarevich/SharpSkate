using System;
using System.Collections.Generic;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Models.EventArgs;
using Sanet.SmartSkating.Models.Geometry;
using Sanet.SmartSkating.Models.Location;

namespace Sanet.SmartSkating.Models.Training
{
    public interface ISession
    {
        event EventHandler<LapEventArgs>? LapPassed;
        string SessionId { get; }
        IList<WayPoint> WayPoints { get; }
        IList<Section> Sectors { get; }
        IList<Lap> Laps { get; }
        Section? BestSector { get; }
        int LapsCount { get; }
        TimeSpan LastLapTime { get; }
        TimeSpan BestLapTime { get; }
        Rink Rink { get; }
        void AddPoint(Coordinate location, DateTime date);
        void AddPoints(IEnumerable<WayPointDto> waypoints);
        void AddSeparatingPoint(WayPointTypes type, DateTime date);

        DateTime StartTime { get; }
        void SetStartTime(DateTime startTime);
        public Coordinate? LastCoordinate { get; }
        bool IsRemote { get; }
        bool IsCompleted { get; }
    }
}
