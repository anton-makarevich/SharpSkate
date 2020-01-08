using System;
using System.Collections.Generic;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Models.Geometry;
using Sanet.SmartSkating.Models.Location;

namespace Sanet.SmartSkating.Models.Training
{
    public interface ISession
    {
        string SessionId { get; }
        IList<WayPoint> WayPoints { get; }
        IList<Section> Sectors { get; }
        Section? BestSector { get; }
        int LapsCount { get; }
        TimeSpan LastLapTime { get; }
        TimeSpan BestLapTime { get; }
        Rink Rink { get; }
        void AddPoint(Coordinate location, DateTime date);
        void AddSeparatingPoint(WayPointTypes type, DateTime date);
        
        DateTime StartTime { get; }
        void SetStartTime(DateTime startTime);
    }
}