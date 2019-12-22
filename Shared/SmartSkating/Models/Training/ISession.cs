using System;
using System.Collections.Generic;
using Sanet.SmartSkating.Models.Geometry;

namespace Sanet.SmartSkating.Models.Training
{
    public interface ISession
    {
        IList<WayPoint> WayPoints { get; }
        IList<Section> Sectors { get; }
        Section? BestSector { get; }
        int LapsCount { get; }
        TimeSpan LastLapTime { get; }
        TimeSpan BestLapTime { get; }
        Rink Rink { get; }
        void AddPoint(Coordinate location, DateTime date);
        void AddSeparatingPoint(Coordinate location, DateTime date, WayPointTypes type);
        
        DateTime StartTime { get; }
        void SetStartTime(DateTime startTime);
    }
}