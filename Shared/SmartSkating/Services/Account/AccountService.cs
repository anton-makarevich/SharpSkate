using System;
using Sanet.SmartSkating.Dto.Models;

namespace Sanet.SmartSkating.Services.Account
{
    public class AccountService:IAccountService
    {
        private readonly IPreferences _preferences;
        private readonly IDeviceInfo _deviceInfo;
        private const string UserIdKey = "userId";
        private const string DeviceIdKey = "deviceId";

        public AccountService(IPreferences preferences, IDeviceInfo deviceInfo)
        {
            _preferences = preferences;
            _deviceInfo = deviceInfo;
        }

        public string UserId => GetUniqueInstallationValue(UserIdKey);

        public DeviceDto GetDeviceInfo() =>
            new DeviceDto
            {
                Id = GetUniqueInstallationValue(DeviceIdKey),
                AccountId = UserId,
                Manufacturer = _deviceInfo.Manufacturer,
                Model = _deviceInfo.Model,
                OsName = _deviceInfo.Platform,
                OsVersion = _deviceInfo.Version
            };

        private string GetUniqueInstallationValue(string key)
        {
            if (key == UserIdKey)
                return "AntonM";
            var userId = _preferences.Get(key, "");

            if (!string.IsNullOrEmpty(userId)) return userId;

            userId = Guid.NewGuid().ToString("N");
            _preferences.Set(key, userId);

            return userId;
        }
    }
}
