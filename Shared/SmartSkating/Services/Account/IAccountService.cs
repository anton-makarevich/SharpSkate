using Sanet.SmartSkating.Dto.Models;

namespace Sanet.SmartSkating.Services.Account
{
    public interface IAccountService
    {
        string UserId { get; set; }
        string DeviceId { get; }
        DeviceDto GetDeviceInfo();
    }
}
