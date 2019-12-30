using System.Threading.Tasks;

namespace Sanet.SmartSkating.Services.Api
{
    public interface IConnectivityService
    {
        Task<bool> IsConnected();
    }
}