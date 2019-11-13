namespace Sanet.SmartSkating.Models
{
    public class Rink
    {
        public Rink(Coordinate start, Coordinate finish)
        {
            Start = start;
            Finish = finish;
        }

        public Coordinate Start { get; }
        public Coordinate Finish { get; }
    }
}