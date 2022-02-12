using System.Collections.Generic;
using System.Linq;
using Sanet.SmartSkating.Dto.Models;

namespace Sanet.SmartSkating.ViewModels.Wrappers
{
    public class SessionViewModel
    {
        public SessionViewModel(SessionDto sessionDto, IEnumerable<TrackDto> tracks)
        {
            Session = sessionDto;
            RinkName = tracks.FirstOrDefault(t=>t.Id == Session.RinkId)?.Name??"Unknown";
            Status = Session.IsCompleted ? "Completed" : "In progress";
        }

        public string StartTime => Session.StartTime.ToString("yy-MM-dd H:mm");
        public string RinkName { get; }
        public string Status { get; }
        public SessionDto Session { get; }
    }
}
