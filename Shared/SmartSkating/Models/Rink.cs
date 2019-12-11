using System.Collections.Generic;
using Sanet.SmartSkating.Models.Location;
using Sanet.SmartSkating.Utils;

namespace Sanet.SmartSkating.Models
{
    public class Rink
    {
        private const int StraightSectorWidthInMeters = 20;
        private const int CornerSectorWidthInMeters = 40;
        private const int CornerMiddleRadiusInMeters = 30;
        public Rink(Coordinate start, Coordinate finish)
        {
            Start = start;
            Finish = finish;

            var longitudeFactor = Start.Longitude.GetLongitudeFactor();
            
            StartLocal = new Point();

            var latitudeToFinish = finish.Latitude - start.Latitude;
            var longitudeToFinish = finish.Longitude - start.Longitude;

            FinishLocal = new Point(
                latitudeToFinish.ToLatitudeDistanceInMeters(),
                longitudeToFinish.ToLongitudeDistanceInMeters(longitudeFactor));

            Finish1KLocal=new Point(FinishLocal.X*0.5,FinishLocal.Y*0.5);

            Start300MLocal = CreateStart300M(StartLocal, FinishLocal);
            Start3KLocal = CreateStart3K(StartLocal, FinishLocal);
            Start1KLocal = new Point(
                (Start300MLocal.X+Start3KLocal.X)*0.5,
                (Start300MLocal.Y+Start3KLocal.Y)*0.5);
            
            FirstSector = CreateStraightSector(StartLocal, FinishLocal);
            ThirdSector = CreateStraightSector(Start300MLocal, Start3KLocal);
            
        }

        public Coordinate Start { get; }
        public Coordinate Finish { get; }
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


        #region Rink elements
        private static Point CreateStart300M(Point beginPoint, Point endPoint)
        {
            var startFinishLine = new Line(beginPoint,endPoint);
            var finishLine = startFinishLine.GetPerpendicularToEnd();
            var finishPoints = finishLine.FindPointsFromBegin(CornerMiddleRadiusInMeters*2);
            foreach (var point in finishPoints)
            {
                if (point.IsLeftFrom(startFinishLine))
                    return point;
            }
            return new Point();
        }

        private static Sector CreateStraightSector(Point beginPoint, Point endPoint)
        {
            var startFinishLine = new Line(beginPoint,endPoint);
            var startLine = startFinishLine.GetPerpendicularToBegin();
            var startPoints = startLine.FindPointsFromBegin(StraightSectorWidthInMeters*0.5);
            var finishLine = startFinishLine.GetPerpendicularToEnd();
            var finishPoints = finishLine.FindPointsFromBegin(StraightSectorWidthInMeters*0.5);
            return new Sector(startPoints, finishPoints);
        }
        
        private static Point CreateStart3K(Point beginPoint, Point endPoint)
        {
            var startFinishLine = new Line(beginPoint,endPoint);
            var startLine = startFinishLine.GetPerpendicularToBegin();
            var startPoints = startLine.FindPointsFromBegin(CornerMiddleRadiusInMeters*2);
            foreach (var point in startPoints)
            {
                if (point.IsLeftFrom(startFinishLine))
                    return point;
            }
            return new Point();
        }
        #endregion
    }
}