using System.Collections.Generic;
using System.Data;
using System.Linq;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Models.Location;
using Sanet.SmartSkating.Models.Training;
using Sanet.SmartSkating.Utils;

namespace Sanet.SmartSkating.Models.Geometry
{
    public class Rink
    {
        private const int StraightSectorWidthInMeters = 20;
        private const int CornerSectorWidthInMeters = 40;
        private const int CornerMiddleRadiusInMeters = 30;

        private readonly double _longitudeFactor;

        public Rink(Coordinate start, Coordinate finish, string id, string name = "Unknown")
        {
            Start = start;
            Finish = finish;
            Name = name;
            Id = id;

            _longitudeFactor = Start.Latitude.GetLongitudeFactor();

            StartLocal = new Point();

            FinishLocal = ToLocalCoordinateSystem(finish);

            Finish1KLocal = new Point(FinishLocal.X * 0.5, FinishLocal.Y * 0.5);

            Start300MLocal = CreateStart300M(StartLocal, FinishLocal);
            Start300M = ToGeoCoordinateSystem(Start300MLocal);
            Start3K = ToGeoCoordinateSystem(Start3KLocal);
            Start3KLocal = CreateStart3K(StartLocal, FinishLocal);
            Start1KLocal = new Point(
                (Start300MLocal.X + Start3KLocal.X) * 0.5,
                (Start300MLocal.Y + Start3KLocal.Y) * 0.5);

            FirstSector = CreateStraightSector(
                StartLocal,
                FinishLocal,
                WayPointTypes.FirstSector);
            ThirdSector = CreateStraightSector(
                Start300MLocal,
                Start3KLocal,
                WayPointTypes.ThirdSector);

            var (secondSectorPoint1, secondSectorPoint2)
                = (FirstSector.FinishLine, ThirdSector.StartLine).FindOppositePoints();
            SecondSector = CreateCornerSector(
                secondSectorPoint1,
                secondSectorPoint2,
                WayPointTypes.SecondSector);

            var (forthSectorPoint1, forthSectorPoint2)
                = (ThirdSector.FinishLine, FirstSector.StartLine).FindOppositePoints();
            FourthSector = CreateCornerSector(
                forthSectorPoint1,
                forthSectorPoint2,
                WayPointTypes.FourthSector);

            var center = (new Line(FirstSector.Center, ThirdSector.Center),
                new Line(SecondSector.Center, FourthSector.Center)).GetIntersection();

            if (center.HasValue)
                Center = center.Value;
            else
                throw new NoNullAllowedException("No center point for sector");
        }

        public string Id { get; set; }
        public Coordinate Start { get; }
        public Coordinate Finish { get; }
        public Coordinate Start300M { get; }
        public Coordinate Start3K { get; }
        public Point StartLocal { get; }
        public Point FinishLocal { get; }
        public Point Finish1KLocal { get; }
        public Point Start300MLocal { get; }
        public Point Start3KLocal { get; }
        public Point Start1KLocal { get; }

        public IList<Sector> Sectors => new List<Sector>()
        {
            FirstSector,
            SecondSector,
            ThirdSector,
            FourthSector
        };

        public Sector FirstSector { get; }
        public Sector SecondSector { get; }
        public Sector ThirdSector { get; }
        public Sector FourthSector { get; }
        public Point Center { get; }
        public string Name { get; }

        #region Rink elements

        private static Point CreateStart300M(Point beginPoint, Point endPoint)
        {
            var startFinishLine = new Line(beginPoint, endPoint);
            var finishLine = startFinishLine.GetPerpendicularToEnd();
            var finishPoints = finishLine.FindPointsFromBegin(CornerMiddleRadiusInMeters * 2);
            foreach (var point in finishPoints)
            {
                if (point.IsLeftFrom(startFinishLine))
                    return point;
            }

            return new Point();
        }

        private static Sector CreateStraightSector(Point beginPoint, Point endPoint, WayPointTypes type)
        {
            var startFinishLine = new Line(beginPoint, endPoint);
            var startLine = startFinishLine.GetPerpendicularToBegin();
            var startPoints = startLine.FindPointsFromBegin(StraightSectorWidthInMeters * 0.5);
            var finishLine = startFinishLine.GetPerpendicularToEnd();
            var finishPoints = finishLine.FindPointsFromBegin(StraightSectorWidthInMeters * 0.5);
            return new Sector(startPoints, finishPoints, type);
        }

        private static Point CreateStart3K(Point beginPoint, Point endPoint)
        {
            var startFinishLine = new Line(beginPoint, endPoint);
            var startLine = startFinishLine.GetPerpendicularToBegin();
            var startPoints = startLine.FindPointsFromBegin(CornerMiddleRadiusInMeters * 2);
            foreach (var point in startPoints)
            {
                if (point.IsLeftFrom(startFinishLine))
                    return point;
            }

            return new Point();
        }

        private static Sector CreateCornerSector(Point start, Point finish, WayPointTypes type)
        {
            var innerSide = new Line(start, finish);

            var startRay = innerSide.GetPerpendicularToBegin();
            var startPoints = startRay.FindPointsFromBegin(CornerSectorWidthInMeters);
            var secondStartPoint = startPoints.First(p => !p.IsLeftFrom(innerSide));
            var startLine = new[] {start, secondStartPoint};

            var finishRay = innerSide.GetPerpendicularToEnd();
            var finishPoints = finishRay.FindPointsFromBegin(CornerSectorWidthInMeters);
            var secondFinishPoint = finishPoints.First(p => !p.IsLeftFrom(innerSide));
            var finishLine = new[] {finish, secondFinishPoint};

            return new Sector(startLine, finishLine, type);
        }

        #endregion

        public Point ToLocalCoordinateSystem(Coordinate coordinate)
        {
            var latitudeDelta = coordinate.Latitude - Start.Latitude;
            var longitudeDelta = coordinate.Longitude - Start.Longitude;

            return new Point(
                longitudeDelta.ToLongitudeDistanceInMeters(_longitudeFactor),
            latitudeDelta.ToLatitudeDistanceInMeters());
        }

        public Coordinate ToGeoCoordinateSystem(Point point)
        {
            var dX = point.X - StartLocal.X;
            var dY = point.Y - StartLocal.Y;

            return new Coordinate(
                Start.Latitude + dY.ToLatitudeDistanceInDegrees(),
                Start.Longitude+ dX.ToLongitudeDistanceInDegrees(_longitudeFactor));
        }
    }
}
