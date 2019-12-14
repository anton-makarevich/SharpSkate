using System;
using Sanet.SmartSkating.Models.Geometry;

namespace Sanet.SmartSkating.Models.Training
{
    public enum WayPointTypes
    {
        Unknown = 0,
        Start = 1,
        Finish1K =2,
        Finish=3,
        Start300M=4,
        Start1K=5,
        Start3K=6,
        FirstSector=7,
        SecondSector=8,
        ThirdSector=9,
        FourthSector=10
    }
    
    public static class WayPointTypesExtensions
    {
        public static WayPointTypes GetSectorTypeBetween(this (WayPointTypes, WayPointTypes) wayPoints)
        {
            var (startType, finishType) = wayPoints;
            return (startType, finishType) switch
            {
                (WayPointTypes.Start, WayPointTypes.Finish) => WayPointTypes.FirstSector,
                (WayPointTypes.Finish, WayPointTypes.Start300M) => WayPointTypes.SecondSector,
                (WayPointTypes.Start300M, WayPointTypes.Start3K) => WayPointTypes.ThirdSector,
                (WayPointTypes.Start3K, WayPointTypes.Start) => WayPointTypes.FourthSector,
                _ => WayPointTypes.Unknown
            };
        }
        
        public static WayPointTypes GetTypeSeparatingSectors(this (WayPointTypes, WayPointTypes) wayPointTypes)
        {
            var (previousType, currentType) = wayPointTypes;
            return (previousType, currentType) switch
            {
                (WayPointTypes.FourthSector,WayPointTypes.FirstSector) => WayPointTypes.Start,
                (WayPointTypes.FirstSector,WayPointTypes.SecondSector) => WayPointTypes.Finish,
                (WayPointTypes.SecondSector,WayPointTypes.ThirdSector) => WayPointTypes.Start300M,
                (WayPointTypes.ThirdSector,WayPointTypes.FourthSector) => WayPointTypes.Start3K,
                _ => WayPointTypes.Unknown
            };
        }
        
        public static Coordinate GetSeparatingPointLocationForType(this WayPointTypes pointType, Rink rink)
        {
            return pointType switch
            {
                WayPointTypes.Start => rink.Start,
                WayPointTypes.Finish => rink.Finish,
                WayPointTypes.Start300M => rink.Start300M,
                WayPointTypes.Start3K => rink.Start3K,
                _ => throw new NotSupportedException($"No coordinate for {pointType} point type")
            };
        }

        public static WayPointTypes GetPreviousSeparationPointType(this WayPointTypes currentType)
        {
            return currentType switch
            {
                WayPointTypes.Start => WayPointTypes.Start3K,
                WayPointTypes.Finish => WayPointTypes.Start,
                WayPointTypes.Start300M => WayPointTypes.Finish,
                WayPointTypes.Start3K => WayPointTypes.Start300M,
                WayPointTypes.FourthSector => WayPointTypes.Start3K,
                WayPointTypes.FirstSector => WayPointTypes.Start,
                WayPointTypes.SecondSector => WayPointTypes.Finish,
                WayPointTypes.ThirdSector => WayPointTypes.Start300M,
                _ => WayPointTypes.Unknown
            };
        }

        public static WayPointTypes GetPreviousSectorType(this WayPointTypes currentType)
        {
            return currentType switch
            {
                WayPointTypes.Start => WayPointTypes.FourthSector,
                WayPointTypes.FirstSector => WayPointTypes.FourthSector,
                WayPointTypes.Finish => WayPointTypes.FirstSector,
                WayPointTypes.SecondSector => WayPointTypes.FirstSector,
                WayPointTypes.Start300M => WayPointTypes.SecondSector,
                WayPointTypes.ThirdSector => WayPointTypes.SecondSector,
                WayPointTypes.Start3K => WayPointTypes.ThirdSector,
                WayPointTypes.FourthSector => WayPointTypes.ThirdSector,
                _ => WayPointTypes.Unknown
            };
        }
    }
}