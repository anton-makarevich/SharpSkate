using Sanet.SmartSkating.Dto.Models;

namespace Sanet.SmartSkating.Services.Account
{
    public interface IAccountService
    {
        string UserId { get; }
        DeviceDto GetDeviceInfo();
    }
}