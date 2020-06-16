using Sanet.SmartSkating.Models.Geometry;
using Sanet.SmartSkating.Models.Training;

namespace Sanet.SmartSkating.Services.Tracking
{
    public class SessionService:ISessionService
    {
        private readonly ISettingsService _settingsService;

        public SessionService(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }
        public ISession CreateSessionForRink(Rink rink)
        {
            return new Session(rink,_settingsService);
        }
    }
}