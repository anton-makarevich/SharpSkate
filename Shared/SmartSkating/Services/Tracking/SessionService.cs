using Sanet.SmartSkating.Models.Geometry;
using Sanet.SmartSkating.Models.Training;

namespace Sanet.SmartSkating.Services.Tracking
{
    public class SessionService:ISessionService
    {
        public ISession CreateSessionForRink(Rink rink)
        {
            return new Session(rink);
        }
    }
}