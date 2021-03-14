using System.Threading.Tasks;

namespace Sanet.SmartSkating.Services.Tracking
{
    public interface ISyncService
    {
        Task ConnectToHub(string accessToken, string url);
    }
}