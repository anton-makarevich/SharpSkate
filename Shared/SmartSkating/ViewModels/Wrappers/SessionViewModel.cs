using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AsyncAwaitBestPractices.MVVM;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Services.Api;

namespace Sanet.SmartSkating.ViewModels.Wrappers
{
    public class SessionViewModel
    {
        private readonly IDataSyncService _dataSyncService;

        public SessionViewModel(SessionDto sessionDto, IEnumerable<TrackDto> tracks, IDataSyncService dataSyncService)
        {
            _dataSyncService = dataSyncService;
            Session = sessionDto;
            RinkName = tracks.FirstOrDefault(t=>t.Id == Session.RinkId)?.Name??"Unknown";
            Status = Session.IsCompleted ? "Completed" : "In progress";
        }

        public string StartTime => Session.StartTime.ToString("yy-MM-dd H:mm");
        public string RinkName { get; }
        public string Status { get; }
        public SessionDto Session { get; }

        public IAsyncValueCommand CompleteSessionCommand => new AsyncValueCommand(CompleteSession);

        private async ValueTask CompleteSession()
        {
            Session.IsCompleted = true;
            await _dataSyncService.SaveAndSyncSessionAsync(Session);
        }
    }
}
