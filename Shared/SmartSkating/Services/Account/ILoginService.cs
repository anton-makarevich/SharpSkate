using System.Threading.Tasks;

namespace Sanet.SmartSkating.Services.Account
{
    public interface ILoginService
    {
        Task<bool> LoginUserAsync(string username, string password);
    }
}