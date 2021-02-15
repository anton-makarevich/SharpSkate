using Sanet.SmartSkating.Models.Geometry;
using Sanet.SmartSkating.Models.Training;

namespace Sanet.SmartSkating.Services.Tracking
{
    public interface ISessionService
    {
        ISession CreateSessionForRink(Rink rink);

        ISession? CurrentSession { get; }
    }
}
