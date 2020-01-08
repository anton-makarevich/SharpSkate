using System;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Models.Geometry;
using Sanet.SmartSkating.Models.Location;

namespace Sanet.SmartSkating.Models.Training
{
    
    public static class WayPointTypesExtensions
    {
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
        
        public static (Point,Coordinate) GetSeparatingPointLocationForType(this WayPointTypes pointType, Rink rink)
        {
            return pointType switch
            {
                WayPointTypes.Start => (rink.StartLocal,rink.Start),
                WayPointTypes.Finish => (rink.FinishLocal,rink.Finish),
                WayPointTypes.Start300M => (rink.Start300MLocal,rink.Start300M),
                WayPointTypes.Start3K => (rink.Start3KLocal,rink.Start3K),
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
        
        public static WayPointTypes GetNextSeparationPointType(this WayPointTypes currentType)
        {
            return currentType switch
            {
                WayPointTypes.Start => WayPointTypes.Finish,
                WayPointTypes.Finish => WayPointTypes.Start300M,
                WayPointTypes.Start300M => WayPointTypes.Start3K,
                WayPointTypes.Start3K => WayPointTypes.Start,
                WayPointTypes.FourthSector => WayPointTypes.Start,
                WayPointTypes.FirstSector => WayPointTypes.Finish,
                WayPointTypes.SecondSector => WayPointTypes.Start300M,
                WayPointTypes.ThirdSector => WayPointTypes.Start3K,
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
        
        public static WayPointTypes GetNextSectorType(this WayPointTypes currentType)
        {
            return currentType switch
            {
                WayPointTypes.Start => WayPointTypes.FirstSector,
                WayPointTypes.FirstSector => WayPointTypes.SecondSector,
                WayPointTypes.Finish => WayPointTypes.SecondSector,
                WayPointTypes.SecondSector => WayPointTypes.ThirdSector,
                WayPointTypes.Start300M => WayPointTypes.ThirdSector,
                WayPointTypes.ThirdSector => WayPointTypes.FourthSector,
                WayPointTypes.Start3K => WayPointTypes.FourthSector,
                WayPointTypes.FourthSector => WayPointTypes.FirstSector,
                _ => WayPointTypes.Unknown
            };
        }

        public static string GetSectorName(this WayPointTypes currentType)
        {
            return currentType switch
            {
                WayPointTypes.FirstSector => "1st",
                WayPointTypes.SecondSector => "2nd",
                WayPointTypes.ThirdSector => "3rd",
                WayPointTypes.FourthSector => "4th",
                _ => "NA"
            };
        }
    }
}