using System.Collections.Generic;
using Sanet.SmartSkating.Models.Location;
using Sanet.SmartSkating.Utils;

namespace Sanet.SmartSkating.Models
{
    public class Rink
    {
        private const int SectorWidthInMeters = 20;
        private const int CornerMiddleRadius = 30;
        public Rink(Coordinate start, Coordinate finish)
        {
            Start = start;
            Finish = finish;

            var longitudeFactor = Start.Longitude.GetLongitudeFactor();
            
            StartLocal = new Point();

            var latitudeToFinish = finish.Latitude - start.Latitude;
            var longitudeToFinish = finish.Longitude - start.Longitude;

            FinishLocal = new Point(latitudeToFinish.ToLatitudeDistanceInMeters(),longitudeToFinish.ToLongitudeDistanceInMeters(longitudeFactor));

            Finish1KLocal=new Point(FinishLocal.X*0.5,FinishLocal.Y*0.5);

            Start300MLocal = CreateStart300M(StartLocal, FinishLocal);
            
            FirstSector = CreateFirstSector(StartLocal, FinishLocal);
        }
        
        public Coordinate Start { get; }
        public Coordinate Finish { get; }
        public Point StartLocal { get; }
        public Point FinishLocal { get; }
        public Point Finish1KLocal { get; }
        public Point Start300MLocal { get; }
        public IList<Sector> Sectors => new List<Sector>(){FirstSector};
        public Sector FirstSector { get; }
        public Sector SecondSector { get; }

        #region Rink elements
        private static Point CreateStart300M(Point beginPoint, Point endPoint)
        {
            var startFinishLine = new Line(beginPoint,endPoint);
            var finishLine = startFinishLine.GetPerpendicularToEnd();
            var finishPoints = finishLine.FindPointsFromBegin(CornerMiddleRadius*2);
            foreach (var point in finishPoints)
            {
                if (point.IsLeftFrom(startFinishLine))
                    return point;
            }
            return new Point();
        }

        private static Sector CreateFirstSector(Point beginPoint, Point endPoint)
        {
            var startFinishLine = new Line(beginPoint,endPoint);
            var startLine = startFinishLine.GetPerpendicularToBegin();
            var startPoints = startLine.FindPointsFromBegin(SectorWidthInMeters*0.5);
            var finishLine = startFinishLine.GetPerpendicularToEnd();
            var finishPoints = finishLine.FindPointsFromBegin(SectorWidthInMeters*0.5);
            return new Sector(startPoints, finishPoints);
        }
        #endregion
    }
}