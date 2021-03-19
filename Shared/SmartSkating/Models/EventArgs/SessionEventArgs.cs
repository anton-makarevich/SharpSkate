using Sanet.SmartSkating.Dto.Models;

namespace Sanet.SmartSkating.Models.EventArgs
{
    public class SessionEventArgs:System.EventArgs
    {
        public SessionDto Session { get; }
        
        public SessionEventArgs(SessionDto session)
        {
            Session = session;
        }
    }
}