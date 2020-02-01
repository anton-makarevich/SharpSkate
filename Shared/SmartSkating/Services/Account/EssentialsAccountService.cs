using System;
using Sanet.SmartSkating.Dto.Models;
using Xamarin.Essentials;

namespace Sanet.SmartSkating.Services.Account
{
    public class EssentialsAccountService:IAccountService
    {
        private const string UserIdKey = "userId";
        private const string DeviceIdKey = "deviceId";
        public string UserId => GetDeviceInfo().Id;//=> GetUniqueInstallationValue(UserIdKey);

        public DeviceDto GetDeviceInfo() =>
            new DeviceDto
            {
                Id =
#if DEBUG
                    "Debug",
#else
                    GetUniqueInstallationValue(DeviceIdKey),
#endif
                Manufacturer = DeviceInfo.Manufacturer,
                Model = DeviceInfo.Model,
                OsName = DeviceInfo.Platform.ToString(),
                OsVersion = DeviceInfo.Version.ToString()
            };

        private static string GetUniqueInstallationValue(string key)
        {
            var userId = Preferences.Get(key, string.Empty);

            if (!string.IsNullOrEmpty(userId)) return userId;

            userId = Guid.NewGuid().ToString("N");
            Preferences.Set(key, userId);

            return userId;
        }
    }
}