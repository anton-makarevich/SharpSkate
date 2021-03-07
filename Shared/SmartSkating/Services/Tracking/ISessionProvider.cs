using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Models.Geometry;
using Sanet.SmartSkating.Models.Training;

namespace Sanet.SmartSkating.Services.Tracking
{
    public interface ISessionProvider
    {
        ISession CreateSessionForRink(Rink rink);

        ISession? CurrentSession { get; }
        void SetActiveSession(SessionDto session, Rink rink);
    }
}
