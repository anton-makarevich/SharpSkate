using System;
using System.Collections.Generic;
using Sanet.SmartSkating.Models.Geometry;

namespace Sanet.SmartSkating.Models.Training
{
    public interface ISession
    {
        IList<WayPoint> WayPoints { get; }
        IList<Section> Sectors { get; }
        int LapsCount { get; }
        TimeSpan LastLapTime { get; }
        Rink Rink { get; }
        void AddPoint(Coordinate location, DateTime date);
        void AddSeparatingPoint(Coordinate location, DateTime date, WayPointTypes type);
    }
}