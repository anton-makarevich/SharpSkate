namespace Sanet.SmartSkating.Dto.Services
{
    public class SessionInfoHelper:ISessionInfoHelper
    {
        public string GetHubNameForSession(string sessionId)
        {
            return $"s{sessionId}";
        }
    }
}