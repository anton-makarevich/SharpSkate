using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Sanet.SmartSkating.Services.Api
{
    public class EssentialsConnectivityService:IConnectivityService
    {
        public Task<bool> IsConnected()
        {
            return Task.Run(() =>
            {
                var current = Connectivity.NetworkAccess;

                return current == NetworkAccess.Internet;
            });
        }
    }
}