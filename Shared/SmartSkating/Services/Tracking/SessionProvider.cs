using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Models.Geometry;
using Sanet.SmartSkating.Models.Training;

namespace Sanet.SmartSkating.Services.Tracking
{
    public class SessionProvider:ISessionProvider
    {
        private readonly ISettingsService _settingsService;

        public SessionProvider(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }
        public ISession CreateSessionForRink(Rink rink)
        {
            CurrentSession = new Session(rink,_settingsService);
            return CurrentSession;
        }

        public void SetActiveSession(SessionDto session, Rink rink)
        {
            CurrentSession = new Session(session.Id, rink, _settingsService);
        }

        public ISession? CurrentSession { get; private set; }
    }
}
