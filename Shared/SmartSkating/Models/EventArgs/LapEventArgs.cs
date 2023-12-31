using Sanet.SmartSkating.Models.Training;

namespace Sanet.SmartSkating.Models.EventArgs
{
    public class LapEventArgs:System.EventArgs
    {
        public LapEventArgs(Lap lap, bool? isBest = null)
        {
            Lap = lap;
            IsBest = isBest;
        }

        public Lap Lap { get; }
        public bool? IsBest { get; }
    }
}